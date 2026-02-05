using System.Collections.Generic;
using System.Linq;
using Katpatat.Networking.Utils;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.VFX;

public class RiderGameManager : MonoBehaviour
{
	public static RiderGameManager Instance;

    [SerializeField] private SplineContainer roadSpline;
    
    [SerializeField] 
    private Rider prefabRider;

    [SerializeField] 
    private RiderIcon prefabRiderIcon;
    
    public List<Rider> riders = new();

    //Simulater player
    [SerializeField] private bool doSimulate;
    float currentSpeed = 0f;
    float currentProgress = 0f;
    float currentLateral = 0f;

    [SerializeField] private float dollySpeed = 0.015f;
    public CinemachineSplineDolly cinemachineSplineDolly;

    public Camera cameraFrom;
    public Camera cameraTo;
    bool sceneStarted = false;
    public GameObject prefabExplosion;

    private void OnEnable() {
        NetworkMessageUtil.OnRiderPosition += RiderPositionReceived;
        NetworkMessageUtil.OnRiderJoined += RiderJoined;
        NetworkMessageUtil.OnRiderLeft += RemoveRider;
        NetworkMessageUtil.OnRiderExplosion += TriggerExplosion;
    }
    private void OnDisable() {
        NetworkMessageUtil.OnRiderPosition -= RiderPositionReceived;
        NetworkMessageUtil.OnRiderJoined -= RiderJoined;
        NetworkMessageUtil.OnRiderLeft -= RemoveRider;
        NetworkMessageUtil.OnRiderExplosion -= TriggerExplosion;
    }
    
    private void Awake()
    {
        Instance = this;

        Debug.Log($"Length of the track: {roadSpline.CalculateLength()}");
    }

    private void Start() 
    {
        if(doSimulate)
            AddRider("noID","Test Subject");
    }

    public void StartCameraFollowSpline()
    {
        var autodolly = cinemachineSplineDolly.AutomaticDolly.Method as SplineAutoDolly.FixedSpeed;
        if (autodolly != null)
            autodolly.Speed = dollySpeed;

        sceneStarted = true;
        DestroyAllRiders();
    }

    public void DestroyAllRiders()
    {
        sceneStarted = true;

        foreach(Rider rider in riders)
        {
            RiderIcon riderIcon = FindObjectsByType<RiderIcon>(FindObjectsSortMode.None).Where(r=>r.rider == rider).FirstOrDefault();
            if(riderIcon!=null)
                Destroy(riderIcon.gameObject);
                
            Destroy(rider.gameObject);
        }
        riders.Clear();
    }

    void Update()
    {
        if(doSimulate)
            SimulatePlayer();
    }

    void SimulatePlayer()
    {
        float incrementSpeed = 0f;
        if(Input.GetKeyDown(KeyCode.UpArrow))
            incrementSpeed+=0.0001f;
        else if(Input.GetKeyDown(KeyCode.DownArrow))
            incrementSpeed-=0.0001f;

        currentSpeed+=incrementSpeed; 
        currentSpeed = Mathf.Clamp(currentSpeed,0f,0.001f);
        currentProgress+=currentSpeed;
        currentProgress = currentProgress%1f;

        float incrementLateral = 0f;
        if(Input.GetKeyDown(KeyCode.LeftArrow))
            incrementLateral-=0.2f;
        else if(Input.GetKeyDown(KeyCode.RightArrow))
            incrementLateral+=0.2f;
        else if(Input.GetKeyDown(KeyCode.Space))
            TriggerExplosion("noID");

        currentLateral+= incrementLateral;
        currentLateral = Mathf.Clamp01(currentLateral);
        
        RiderPositionReceived("noID",currentProgress,currentLateral, "Test Subject");
    }

    private void RiderPositionReceived(string id, float progress, float lateralPosition, string nickname) 
    {
        if(sceneStarted)
            return;
        
        var rider = riders.FirstOrDefault(r=> r.Id == id);

        if(!rider)
            rider = AddRider(id, nickname);
        
        rider.SetProgressOnTrack(progress);
        rider.SetLateralPosition(lateralPosition);
    }

    private void RiderJoined(string id, string nickname) 
    {
        if(sceneStarted)
            return;    

        var rider = riders.FirstOrDefault(r=> r.Id == id);

        if (!rider) 
        {
            AddRider(id,nickname);
        }
        else if (!rider.gameObject.activeSelf)
        {
            rider.gameObject.SetActive(true);
        }
    }
    
    private Rider AddRider(string id, string nickname)
    {
        var newRider = Instantiate(prefabRider, Vector3.up * 15f, Quaternion.identity, transform);
        newRider.Initialize(id, roadSpline, nickname);
        riders.Add(newRider);

        RiderIcon riderIcon = Instantiate(prefabRiderIcon,newRider.transform.position,Quaternion.identity,transform).GetComponent<RiderIcon>();
        riderIcon.SetRider(newRider);
        
        return newRider;
    }
    
    private void RemoveRider(string id) 
    {
        var rider = riders.FirstOrDefault(r=> r.Id == id);

        if (!rider)
            return;
        
        RiderIcon riderIcon = FindObjectsByType<RiderIcon>(FindObjectsSortMode.None).Where(r=>r.rider == rider).FirstOrDefault();
        if(riderIcon!=null)
            Destroy(riderIcon.gameObject);

        riders.Remove(rider);

        Destroy(rider.gameObject);

    }

    void TriggerExplosion(string id)
    {
        var rider = riders.FirstOrDefault(r=> r.Id == id);

        if (!rider)
            return;

        VisualEffect r = Instantiate(prefabExplosion,rider.transform.position,Quaternion.identity).GetComponent<VisualEffect>();
        r.Play();
    }
}
