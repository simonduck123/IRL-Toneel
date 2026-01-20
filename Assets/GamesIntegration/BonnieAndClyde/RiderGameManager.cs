using System.Collections.Generic;
using System.Linq;
using Katpatat.Networking.Utils;
using UnityEngine;
using UnityEngine.Splines;

public class RiderGameManager : MonoBehaviour
{
	public static RiderGameManager Instance;

    [SerializeField] private SplineContainer roadSpline;
    
    [SerializeField] 
    private Rider prefabRider;
    
    public List<Rider> riders = new();

    //Simulater player
    bool doSimulate = true;
    float currentSpeed = 0f;
    float currentProgress = 0f;
    float currentLateral = 0f;

    private void OnEnable() {
        NetworkMessageUtil.OnRiderPosition += RiderPositionReceived;
        NetworkMessageUtil.OnPlayerRemove += RemoveRider;
    }
    private void OnDisable() {
        NetworkMessageUtil.OnRiderPosition -= RiderPositionReceived;
        NetworkMessageUtil.OnPlayerRemove -= RemoveRider;
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
    
    private Rider AddRider(string id)
    {
        var newRider = Instantiate(prefabRider, Vector3.up * 15f, Quaternion.identity, transform);
        newRider.Initialize(id, roadSpline);
        riders.Add(newRider);
        
        return newRider;
    }
    
    private void RemoveRider(string id) {
        var rider = riders.FirstOrDefault(r=> r.Id == id);

        if (rider && rider.gameObject.activeSelf) {
            rider.gameObject.SetActive(false);
        }
    }
}
