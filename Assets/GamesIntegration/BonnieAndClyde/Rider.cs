//"Vivian's Bike" (https://skfb.ly/ooJrV) by Mace is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).

using UnityEngine;
using UnityEngine.Splines;

public class Rider : MonoBehaviour
{ 
    public string Id { get; private set; }
    
    public void SetProgressOnTrack(float p) => targetProgressOnTrack = p%1f;
    public void SetLateralPosition(float p) => currentLateralTarget = p;
    public void SetSpeedProgress(float s) => speedProgress = s;

    public float progressOnTrack;
    float targetProgressOnTrack;
    
    [Range(-1,1)]
    public float currentLateralTarget;
    public float widthRoad = 2f;
    public float offsetUp = 1f;
    
    public float speedProgress = 0f;
    
    private bool dividedBy1000 = false;
    public bool useSpeed = false;

    private float currentLateralPosition;
    
    public SplineContainer spline;
    public string nickname;

    public void Initialize(string id, SplineContainer splineContainer, string nickname) {
        Id = id;
        spline = splineContainer;
        this.nickname = nickname;

        currentLateralPosition = currentLateralTarget;
    }
    
    // private void Start()
    // {
    //     //TODO: Assign it in a cleaner way
    //     spline = FindFirstObjectByType<SplineContainer>();
    //
    //     //TODO: Remove random values at start
    //     progressOnTrack = Random.value*1000f;
    //     targetLateralPosition = Random.value;
    //     speedProgress = Random.Range(10f,100f);
    // }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
            SetLateralPosition(0.75f);
        }
        
        if (Input.GetKeyDown(KeyCode.O)) {
            SetLateralPosition(0.25f);
        }
        
        if (!spline)
            return;

        //progressOnTrack += (useSpeed ? 1f : 0f) * speedProgress * Time.deltaTime / (dividedBy1000 ? 1f : 1000f);
        if(targetProgressOnTrack<progressOnTrack)
            progressOnTrack-=1f;

        progressOnTrack = Mathf.Lerp(progressOnTrack,targetProgressOnTrack,RiderGameManager.Instance.lerpPositionProgress);

        var progress = Mod(progressOnTrack / (dividedBy1000 ? 1000f : 1f),1f);

        Vector3 posOnSpline = spline.EvaluatePosition(progress);
        Vector3 direction = spline.EvaluateTangent(progress);
        Vector3 up = spline.EvaluateUpVector(progress);
        var perpendicular = Vector3.Cross(up, direction).normalized;
        
        if(Mathf.Abs(currentLateralPosition-currentLateralTarget)<0.01f)
            currentLateralPosition = currentLateralTarget;
        else
            currentLateralPosition = Mathf.Lerp(currentLateralPosition,currentLateralTarget,5f*Time.deltaTime);

        var pos = posOnSpline + perpendicular * (currentLateralPosition * (widthRoad / 4)) + up * offsetUp;
        
        
        transform.position = Vector3.Lerp(transform.position,pos,0.2f);

        transform.LookAt(pos + direction, up);
    }

    private float Mod(float value, float max) => (value % max + max) % max;
}
