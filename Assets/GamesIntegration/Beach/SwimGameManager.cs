using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Katpatat.Networking.Utils;
using Random = UnityEngine.Random;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SwimGameManager : MonoBehaviour
{
    public static SwimGameManager Instance;
    
    [SerializeField] 
    private Swimmer prefabSwimmer;
    [SerializeField] 
    private GameObject prefabShark;
    
    public List<Swimmer> swimmers = new();
    public GameObject swimmingArea;
    public AnimationCurve jumpCurve;
    public AnimationCurve fallCurve;
    public AnimationCurve deathCurve;
 public AnimationCurve diveCurve;
    private Vector3 prevPos,prevRot,prevScal;

    //Simulated Player
    Vector2 currentCoo = new Vector2(0.5f,0.5f);

    public bool drawGizmo = false;
    
    private void OnEnable() 
    {
        NetworkMessageUtil.OnSwimLocation += SwimLocationReceived;
        NetworkMessageUtil.OnPlayerRemove += RemovePlayer;
        NetworkMessageUtil.OnSwimPlayerAction += HandlePlayerAction;
    }
    private void OnDisable() 
    {
        NetworkMessageUtil.OnSwimLocation -= SwimLocationReceived;
        NetworkMessageUtil.OnPlayerRemove -= RemovePlayer;
        NetworkMessageUtil.OnSwimPlayerAction -= HandlePlayerAction;
    }
    
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        prevPos = swimmingArea.transform.position;
        prevRot = swimmingArea.transform.localEulerAngles;
        prevScal = swimmingArea.transform.localScale;
    }

    void Update()
    {
        SimulatePlayerMotion();
        SimulatePlayerAction();

        if(prevPos!=swimmingArea.transform.position ||
            prevRot != swimmingArea.transform.localEulerAngles ||
            prevScal != swimmingArea.transform.localScale)
        {
            foreach(Swimmer s in swimmers)
                s.SetTargetPos();
        }
    }

    void SimulatePlayerMotion()
    {
        Vector2 add = Vector2.zero;

        if(Input.GetKeyDown(KeyCode.LeftArrow))
            add.x-=0.05f;
        else if(Input.GetKeyDown(KeyCode.RightArrow))
            add.x+=0.05f;
        else if(Input.GetKeyDown(KeyCode.UpArrow))
            add.y-=0.05f;
        else if(Input.GetKeyDown(KeyCode.DownArrow))
            add.y+=0.05f;

        if(add.magnitude==0f)
            return;
        
        currentCoo+=add;
        
        if(currentCoo.x<0f||currentCoo.x>1f||currentCoo.y<0f||currentCoo.y>1f)
        {
            currentCoo = new Vector2(Mathf.Clamp01(currentCoo.x),Mathf.Clamp01(currentCoo.y));
            RemovePlayer("noID");
            return;
        }

        currentCoo = new Vector2(Mathf.Clamp01(currentCoo.x),Mathf.Clamp01(currentCoo.y));
        SwimLocationReceived("noID",currentCoo.x,currentCoo.y);
            
    }

    void SimulatePlayerAction()
    {
        string act = "";
        if(Input.GetKeyDown(KeyCode.I))
            act = "swimJump";
        else if(Input.GetKeyDown(KeyCode.O))
            act = "swimSpiral";
        else if(Input.GetKeyDown(KeyCode.P))
            act = "swimDive";

        if(act=="")
            return;

        HandlePlayerAction("noID",act);
    }   
    
    private void SwimLocationReceived(string id, float x, float y) 
    {
        OnReceiveCoordinatesPlayer(id, new Vector2(x, y));
    }
    
    private void RemovePlayer(string id) 
    {
        Swimmer swimmer = swimmers.FirstOrDefault(s=>s.data.id==id);

        if (swimmer!=null && swimmer.isConnected)
            swimmer.OnPlayerLeaves();
    }
    
    private void HandlePlayerAction(string id, string actionName) 
    {
        Swimmer swimmer = swimmers.FirstOrDefault(s=>s.data.id==id);

        if (swimmer!=null)
            swimmer.HandleAction(actionName);
    }

    public Vector3 GetRemappedPosition(Vector2 normCoordinates)
    {
        float x = Mathf.Lerp(-5f, 5f, normCoordinates.x);
        float z = Mathf.Lerp(5f, -5f, normCoordinates.y);
        Vector3 localPoint = new Vector3(x, 0f, z);

        return swimmingArea.transform.TransformPoint(localPoint);
    }

    public void OnReceiveCoordinatesPlayer(string id, Vector2 normCoordinates)
    {
        Swimmer swimmer = swimmers.FirstOrDefault(s=>s.data.id==id);
        
        if(!swimmer)
        {
            AddNewPlayer(id,normCoordinates);
            return;
        }
        
        swimmer.SetCoordinates(normCoordinates);

        if(!swimmer.isConnected)
            swimmer.OnPlayerJoins();
    }

    public void OnReceiveJumpPlayer(string id)
    {
        Swimmer swimmer = swimmers.FirstOrDefault(s=>s.data.id==id);
        if(swimmer==null)
        {
            AddNewPlayer(id,RandomCoordinates());
            return;
        }

        swimmer.Jump();
    }

    public void AddNewPlayer(string id, Vector2 coordinates)
    {
        var newSwimmer = Instantiate(prefabSwimmer, Vector3.up * 15f, Quaternion.identity, transform);
        newSwimmer.SetData(id,coordinates);
        newSwimmer.OnPlayerJoins();
        swimmers.Add(newSwimmer);
    }

    public void KillPlayer(string id)
    {
        Swimmer swimmer = swimmers.FirstOrDefault(s=>s.data.id==id);
        if(swimmer==null)
            return;
        swimmer.Die();
    }

    public Vector2 RandomCoordinates() => new Vector2(Random.Range(0.15f,0.85f),Random.Range(0.15f,0.75f));

    public void SpawnShark(Swimmer targetSwimmer)
    {
        Vector3 pos = targetSwimmer.transform.position+Vector3.up*-20f;
        Shark newShark = Instantiate(prefabShark,pos,Quaternion.identity).GetComponent<Shark>();
        newShark.SetTarget(targetSwimmer);
    }

    public void RemoveSwimmerFromArray(Swimmer swimmer)
    {
        int indexOf = swimmers.IndexOf(swimmer);
        if(indexOf<0)
            return;
        swimmers.Remove(swimmer);
    }

    public Swimmer GetSwimmerFromId(string id)
    {
        return swimmers.FirstOrDefault(s=>s.data.id==id);
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        if(!drawGizmo)
            return;
            
        Vector3 topLeft = swimmingArea.transform.TransformPoint(new Vector3(-5f, 0f, 5f));
        Gizmos.DrawLine(topLeft,topLeft+Vector3.up*6f);
        Handles.Label(topLeft+Vector3.up*10f,"0;0");
    }
    #endif
}
