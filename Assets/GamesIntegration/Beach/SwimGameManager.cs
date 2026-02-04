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
    public SwimArea[] swimAreas;
    public AnimationCurve jumpCurve;
    public AnimationCurve fallCurve;
    public AnimationCurve deathCurve;
    public AnimationCurve diveCurve;
    

    //Simulated Player
    Vector2 currentCoo = new Vector2(0.5f,0.5f);
    private Vector3 prevPos,prevRot,prevScal;

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
        SwimArea swimArea = swimAreas[0];
        prevPos = swimArea.referencePlane.transform.position;
        prevRot = swimArea.referencePlane.transform.localEulerAngles;
        prevScal = swimArea.referencePlane.transform.localScale;
    }

    void Update()
    {
        SimulatePlayerMotion();
        SimulatePlayerAction();
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
            RemovePlayer(swimAreas[0].id,"noID");
            return;
        }

        currentCoo = new Vector2(Mathf.Clamp01(currentCoo.x),Mathf.Clamp01(currentCoo.y));
        SwimLocationReceived(swimAreas[0].id,"noID",currentCoo.x,currentCoo.y);
            
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

        HandlePlayerAction(swimAreas[0].id,"noID",act);
    }   
    
    private void SwimLocationReceived(string idModule, string idPlayer, float x, float y) 
    {
        SwimArea swimArea = GetSwimArea(idModule);
        if(swimArea==null)
            return;

        Vector2 normCoordinates = new Vector2(x,y);
        Swimmer swimmer = swimmers.FirstOrDefault(s=>s.data.id==idPlayer);

        if(!swimmer)
        {
            AddNewPlayer(swimArea,idPlayer,normCoordinates);
            return;
        }
        
        swimmer.SetCoordinates(normCoordinates);

        if(!swimmer.isConnected)
            swimmer.OnPlayerJoins();
    }
    
    private void RemovePlayer(string idModule, string idPlayer) 
    {
        Swimmer swimmer = swimmers.FirstOrDefault(s=>s.data.id==idPlayer);

        if (swimmer!=null && swimmer.isConnected)
            swimmer.OnPlayerLeaves();
    }
    
    private void HandlePlayerAction(string idModule, string idPlayer, string actionName) 
    {
        Swimmer swimmer = swimmers.FirstOrDefault(s=>s.data.id==idPlayer);

        if (swimmer!=null)
            swimmer.HandleAction(actionName);
    }

    public Vector3 GetRemappedPosition(SwimArea swimArea, Vector2 normCoordinates)
    {
        float x = Mathf.Lerp(-5f, 5f, normCoordinates.x);
        float z = Mathf.Lerp(5f, -5f, normCoordinates.y);
        Vector3 localPoint = new Vector3(x, 0f, z);

        return swimArea.referencePlane.transform.TransformPoint(localPoint);
    }

    public void AddNewPlayer(SwimArea swimArea, string id, Vector2 coordinates)
    {
        var newSwimmer = Instantiate(prefabSwimmer, Vector3.up * 15f, Quaternion.identity, transform);
        newSwimmer.SetData(id,coordinates,swimArea);
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
            
        foreach(SwimArea swimArea in swimAreas)
        {
            Vector3 topLeft = swimArea.referencePlane.transform.TransformPoint(new Vector3(-5f, 0f, 5f));
            Gizmos.DrawLine(topLeft,topLeft+Vector3.up*6f);
            Handles.Label(topLeft+Vector3.up*10f,"0;0"); 
        }  
    }
    #endif

    public SwimArea GetSwimArea(string id)
    {
        return swimAreas.FirstOrDefault();
    }
}

[System.Serializable]
public class SwimArea
{
    public GameObject referencePlane;
    public string id;
}