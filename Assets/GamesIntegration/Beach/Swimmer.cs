//Lowpoly Chibi Character by mehreen1919 [CC-BY] via Poly Pizza

using System.Collections;
using System.Linq;
using Katpatat.Networking;
using UnityEngine;
using UnityEngine.Networking;

public class Swimmer : MonoBehaviour
{
    public SwimmerData data;
    public Shark shark;
    public Rigidbody rb;
    
    public bool drowning;

    private float rand;

    bool isJumping = false;
    bool isSwirling = false;
    bool isDiving = false;

    private float posY;
    private float heightDropFrom = 20f;
    
    private bool isDying;
    private float dyingProgress;
    private bool startedDrawning = false;

    private Vector3 targetPos;
    private Vector3 motion = Vector3.zero;
    public Animator animControl;
    public bool isConnected = false;

    public float buoyancy = 40f;
    public float gravity = 30f;
    public float waterDrag = 4f;
    public float waterAngularDrag = 4f;
    public float airDrag = 0.1f;
    public float airAngularDrag = 0.1f;
    public float waterJumpImpulse = 8f;
    public float forceSwirl = 300f;

    public float maxAcceleration = 30f;
    public float maxSpeedWater = 6f;
    public float maxSpeedAir = 6f;
    public float slowRadius = 2f;

    public float torqueStrength = 100f;
    public float torqueDamping = 0.1f;
    float timeAlive=0f;
    public GameObject bodyHolder;

    float rotatingSens = 0f;
    float progressRotating = 0f;
    bool isRotating = false;
    bool inverting = false;

    bool isTwirling;
    float timerTwirl;

    public GameObject body2D;
    public GameObject quadBody;
    float timerUpAndDown;

    private void Start()
    {
        transform.localScale = Vector3.zero;
        rand = Random.Range(-1f,1f);
        LoadBodyPicture();
    }

    public void LoadBodyPicture()
    {
        
    }

    private void Update()
    {
        ManageBody2D();


        timeAlive=Mathf.Clamp01(timeAlive+Time.deltaTime);
        transform.localScale = Vector3.one*3f*timeAlive;

        if(isDying)
        {
            dyingProgress=Mathf.Clamp01(dyingProgress+Time.deltaTime);
            if(dyingProgress== 1f)
            {
                if(shark)
                {
                    shark.gameObject.SetActive(false);
                    Destroy(shark.gameObject);
                    shark = null;
                }

                gameObject.SetActive(false);

                NameSwimmer nameSwimmer = FindObjectsByType<NameSwimmer>(FindObjectsSortMode.None).Where(r=>r.swimmer == this).FirstOrDefault();
                if(nameSwimmer!=null)
                    Destroy(nameSwimmer.gameObject);


                SwimGameManager.Instance.RemoveSwimmerFromArray(this);
                Destroy(gameObject);
            }
        }

         float addTwirl = 0f;
        if(isTwirling)
        {
            timerTwirl = Mathf.Clamp01(timerTwirl+Time.deltaTime/3f);
            addTwirl = SwimGameManager.Instance.twirlCurve.Evaluate(timerTwirl)*360f*3f;
            if(timerTwirl==1f)
            {
                addTwirl = 0f;
                isTwirling = false;
            } 
        }

        if(inverting)
            return;

        if(rotatingSens>0f)
        {
            progressRotating = Mathf.Clamp01(progressRotating+Time.deltaTime);
            if(progressRotating==1f)
            {
                inverting = true;
                StartCoroutine(WaitAndInvertSensDive());
            }         
        }
        else if(rotatingSens<0f)
        {
            progressRotating = Mathf.Clamp01(progressRotating-Time.deltaTime);
            if(progressRotating==0f)
            {
                rotatingSens = 0f;
                isDiving = false;
            }    
        }

       

        

        //bodyHolder.transform.localEulerAngles = Vector3.right*180f*SwimGameManager.Instance.diveCurve.Evaluate(progressRotating);
        quadBody.transform.localEulerAngles = new Vector3(0f,180f+addTwirl,SwimGameManager.Instance.diveCurve.Evaluate(progressRotating)*180f);
    }

    public void ManageBody2D()
    {
        body2D.transform.rotation = Quaternion.LookRotation(-SwimGameManager.Instance.cam.transform.forward, Vector3.up);
    }

    IEnumerator WaitAndInvertSensDive()
    {
        yield return new WaitForSeconds(0.25f);
        rotatingSens = -1f;
        inverting = false;
    }


    void FixedUpdate()
    {
        float depth = data.swimArea.referencePlane.transform.position.y - transform.position.y;

        bool inAir = depth<=0f;



        if(!inAir)
        {
            timerUpAndDown+=Time.deltaTime;
            Vector3 pos = quadBody.transform.localPosition;
            pos.y = Mathf.Lerp(-0.1f,0.02f,Mathf.Sin(timerUpAndDown*2f)*0.5f+0.5f);
            quadBody.transform.localPosition = pos;
        }



        if(inAir|| isDying)
        {
            rb.linearDamping = airDrag;
            rb.angularDamping = airAngularDrag;
            rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);   
        }
        else
        {
            rb.linearDamping = waterDrag;
            rb.angularDamping = waterAngularDrag;
            float buoyancyForce = buoyancy * Mathf.Clamp01(depth);
            rb.AddForce(Vector3.up * buoyancyForce, ForceMode.Acceleration);
            isJumping = false;
            
            if(isSwirling)
            {
                isSwirling = false;
                rb.angularVelocity = Vector3.zero;
            }
        }

        if(isDying)
        {
            rb.linearVelocity = new Vector3(0f,rb.linearVelocity.y,0f);
        }


         Vector3 currentPos = transform.position;
            currentPos.y = targetPos.y;

            Vector3 toTarget = targetPos - currentPos;
            float distance = toTarget.magnitude;

            if (distance > 0.01f)
            {
                Vector3 desiredVelocity;

                if (distance > slowRadius)
                {
                    desiredVelocity = toTarget.normalized * (inAir?maxSpeedAir:maxSpeedWater);
                }
                else
                {
                    float t = distance / slowRadius;
                    desiredVelocity = toTarget.normalized * ((inAir?maxSpeedAir:maxSpeedWater) * t);
                }

                Vector3 steering = desiredVelocity - rb.linearVelocity;

                Vector3 acceleration = Vector3.ClampMagnitude(steering, maxAcceleration);

                rb.AddForce(acceleration, ForceMode.Acceleration);
            }

        if(!inAir && !isSwirling)
        {
            Vector3 velocity = rb.linearVelocity;
            velocity.y = 0f;

            if (velocity.sqrMagnitude < 0.01f)
                return;

            Vector3 desiredForward = velocity.normalized;
            Vector3 currentForward = transform.forward;
            currentForward.y = 0f;

            float angle = Vector3.SignedAngle(currentForward, desiredForward, Vector3.up);

            float angularVelocityY = rb.angularVelocity.y;
            float torque = angle * torqueStrength - angularVelocityY * torqueDamping;

            rb.AddTorque(Vector3.up * torque, ForceMode.Acceleration);

        }


        if(rb.linearVelocity.magnitude>300f)
            rb.linearVelocity = 300f*rb.linearVelocity.normalized;
    }

    public void Die()
    {
        if(isDying)
            return;

        //SwimGameManager.Instance.SpawnShark(this);
        isDying=true;
        OnPlayerLeaves();
    }


    public void SetData(string id, Vector2 coordinates, SwimArea swimArea, string nickname)
    {
        data = new SwimmerData(id);
        data.coordinates = coordinates;
        data.swimArea = swimArea;
        data.nickname = nickname;

        if(data.id=="noID")
            return;

        StartCoroutine(LoadImage());
    }

    IEnumerator LoadImage()
    {
        string urlPicture =  $"{NetworkClient.config.serverConfig.baseUrl}/m/wh/{NetworkClient.config.serverConfig.characterCreatorModuleId}/body/{data.id}/normal/normal/normal";
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(urlPicture))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Image download failed: " + request.error);
                yield break;
            }

            Texture2D texture = DownloadHandlerTexture.GetContent(request);

            quadBody.GetComponent<Renderer>().material.SetTexture("_MainTex",texture);
        }
    }

    public void SetCoordinates(Vector2 newCoordinates)
    {
        data.coordinates = newCoordinates;
        SetTargetPos();
        /*if(data.coordinates.x>1f || data.coordinates.x<0f || data.coordinates.y>1f || data.coordinates.y<0f)
            Die();*/
    }

    public void TeleportAboveCurrentCoordinates()
    {
        SetTargetPos();
        Vector2 randCircle = Random.insideUnitCircle*Random.Range(0.5f,1.5f);

        Vector3 pos = targetPos+new Vector3(randCircle.x,0f,randCircle.y);
        pos.y = data.swimArea.referencePlane.transform.position.y+heightDropFrom;
        transform.position = pos;
        rb.linearVelocity = Vector3.zero;
    }

    public void SetTargetPos()
    {
        targetPos = SwimGameManager.Instance.GetRemappedPosition(data.swimArea,data.coordinates);
    }
    
    public void HandleAction(string actionName) 
    {
        switch (actionName) 
        {
            case "swimJump":
                Jump();
                break;
            case "swimSpiral":
                Swirl();
                break;
            case "swimDive":
                Dive();
                break;
            default:
                Debug.LogWarning($"No action defined for: {actionName}");
                break;
        }
    }

    public void Jump()
    {
        if(isDying)
            return;


        float depth = data.swimArea.referencePlane.transform.position.y - transform.position.y;
        bool isUnderwater = depth > 0f;

        if (isUnderwater)
        {
            rb.AddForce(Vector3.up * waterJumpImpulse, ForceMode.VelocityChange);
            isJumping = true;
        }
    }

    public void Swirl()
    {
        if(isDying)
            return;

        if(isSwirling)
            return;

        if(isTwirling)
            return;

        //rb.AddForce(Vector3.up * waterJumpImpulse*0.25f, ForceMode.VelocityChange);
        //rb.AddTorque(Vector3.up * forceSwirl, ForceMode.Force);
        timerTwirl = 0f;
        isTwirling = true;
        isSwirling = true;
    }

    public void Dive()
    {
        if(isDiving)
            return;

        isDiving = true;
        rotatingSens = 1f;
    }

    public void OnPlayerJoins()
    {
         TeleportAboveCurrentCoordinates();
         isConnected = true;
         isDying = false;
    }

    public void OnPlayerLeaves()
    {
        isConnected = false;
        isDying = true;
        data.id = "dying";
        SwimGameManager.Instance.SpawnShark(this);

        NameSwimmer nameSwimmer = FindObjectsByType<NameSwimmer>(FindObjectsSortMode.None).Where(r=>r.swimmer == this).FirstOrDefault();
        if(nameSwimmer!=null)
            nameSwimmer.ShowName(false);
    }
}

[System.Serializable]
public class SwimmerData
{
    public string id;
    public Vector2 coordinates;
    public SwimArea swimArea;
    public string nickname = "Test Name";

    public SwimmerData(string id)
    {
        this.id = id;
    }
}