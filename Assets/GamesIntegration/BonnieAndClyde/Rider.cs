//"Vivian's Bike" (https://skfb.ly/ooJrV) by Mace is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).

using UnityEngine;
using UnityEngine.Splines;

public class Rider : MonoBehaviour
{ 
    public string Id { get; private set; }
    
    public float fallbackLerpSpeed = 10f;
    Vector3 basePosition;
    private Vector3 lastPos;
    private Vector3 nextPos;
    private float lastUpdateTime;
    private float interpolationDuration;

    public float progressOnTrack;

    [Range(-1,1)]
    public float currentLateralTarget;
    public float widthRoad = 2f;
    public float offsetUp = 1f;
    
    public float speedProgress = 0f;
    
    public bool useSpeed = false;

    private float currentLateralPosition;
    
    public SplineContainer spline;
    public string nickname;
    public Transform headPosition;

    private void Start()
    {
        lastPos = nextPos = basePosition = transform.position;
        lastUpdateTime = Time.time;
        interpolationDuration = 0.1f;
    }

    public void Initialize(string id, SplineContainer splineContainer, string nickname) 
    {
        Id = id;
        spline = splineContainer;
        this.nickname = nickname;
        currentLateralPosition = currentLateralTarget;
    }

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

        float t = (Time.time - lastUpdateTime) / interpolationDuration;


        if (t <= 1f)
            basePosition = Vector3.Lerp(lastPos, nextPos, t);
        else
            basePosition = Vector3.Lerp(basePosition, nextPos, Time.deltaTime * fallbackLerpSpeed);

        Vector3 direction = spline.EvaluateTangent(progressOnTrack);
        Vector3 up = spline.EvaluateUpVector(progressOnTrack);
        var perpendicular = Vector3.Cross(up, direction).normalized;
        
        if(Mathf.Abs(currentLateralPosition-currentLateralTarget)<0.01f)
            currentLateralPosition = currentLateralTarget;
        else
            currentLateralPosition = Mathf.Lerp(currentLateralPosition,currentLateralTarget,5f*Time.deltaTime);

        Vector3 pos = basePosition + perpendicular * (currentLateralPosition * (widthRoad / 4)) + up * offsetUp;
        
        
        transform.position = pos;

        transform.LookAt(pos + direction, up);
    }

    public void SetProgressOnTrack(float p)
    {
        progressOnTrack = p;
        lastPos = nextPos;
        nextPos = spline.EvaluatePosition(p);
        float now = Time.time;
        interpolationDuration = Mathf.Max(0.01f, now - lastUpdateTime);
        lastUpdateTime = now;
    }

    public void SetLateralPosition(float p) => currentLateralTarget = p;
    public void SetSpeedProgress(float s) => speedProgress = s;
}
