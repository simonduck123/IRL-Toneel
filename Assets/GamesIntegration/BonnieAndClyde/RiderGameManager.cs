using System.Collections.Generic;
using System.Linq;
using Katpatat.Networking.Utils;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

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

    public Camera cam;

    private void OnEnable() {
        NetworkMessageUtil.OnRiderPosition += RiderPositionReceived;
        NetworkMessageUtil.OnRiderJoined += RiderJoined;
        NetworkMessageUtil.OnRiderLeft += RemoveRider;
    }
    private void OnDisable() {
        NetworkMessageUtil.OnRiderPosition -= RiderPositionReceived;
        NetworkMessageUtil.OnRiderJoined -= RiderJoined;
        NetworkMessageUtil.OnRiderLeft -= RemoveRider;
    }
    
    private void Awake()
    {
        Instance = this;

        Debug.Log($"Length of the track: {roadSpline.CalculateLength()}");
    }

    private void Start() 
    {
        if(doSimulate)
            AddRider("noID");
    }

    public void StartCameraFollowSpline()
    {
        var autodolly = cinemachineSplineDolly.AutomaticDolly.Method as SplineAutoDolly.FixedSpeed;
        if (autodolly != null)
            autodolly.Speed = dollySpeed;
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

        currentLateral+= incrementLateral;
        currentLateral = Mathf.Clamp01(currentLateral);
        
        RiderPositionReceived("noID",currentProgress,currentLateral);
    }

    private void RiderPositionReceived(string id, float progress, float lateralPosition) {
        var rider = riders.FirstOrDefault(r=> r.Id == id);
        
        if(!rider)
        {
            rider = AddRider(id);
        }
        
        rider.SetProgressOnTrack(progress);
        rider.SetLateralPosition(lateralPosition);
    }

    private void RiderJoined(string id) {
        var rider = riders.FirstOrDefault(r=> r.Id == id);

        if (!rider) {
            AddRider(id);
        }
        else if (!rider.gameObject.activeSelf){
            rider.gameObject.SetActive(true);
        }
    }
    
    private Rider AddRider(string id)
    {
        var newRider = Instantiate(prefabRider, Vector3.up * 15f, Quaternion.identity, transform);
        newRider.Initialize(id, roadSpline);
        riders.Add(newRider);

        RiderIcon riderIcon = Instantiate(prefabRiderIcon,newRider.transform.position,Quaternion.identity,transform).GetComponent<RiderIcon>();
        riderIcon.SetRider(newRider);
        
        return newRider;
    }
    
    private void RemoveRider(string id) {
        var rider = riders.FirstOrDefault(r=> r.Id == id);

        if (rider && rider.gameObject.activeSelf) {
            rider.gameObject.SetActive(false);
        }
    }
}
