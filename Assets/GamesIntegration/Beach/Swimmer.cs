using System.Collections;
using UnityEngine;

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

    private void Start()
    {
        transform.localScale = Vector3.zero;
        rand = Random.Range(-1f,1f);
    }

    private void Update()
    {
        timeAlive=Mathf.Clamp01(timeAlive+Time.deltaTime);
        transform.localScale = Vector3.one*3f*timeAlive;

        if(isDying)
        {
            dyingProgress=Mathf.Clamp01(dyingProgress+Time.deltaTime);
            if(dyingProgress> 1f)
            {
                if(shark)
                {
                    shark.gameObject.SetActive(false);
                    Destroy(shark.gameObject);
                    shark = null;
                }

                gameObject.SetActive(false);
                SwimGameManager.Instance.RemoveSwimmerFromArray(this);
                Destroy(gameObject);
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

        bodyHolder.transform.localEulerAngles = Vector3.right*180f*SwimGameManager.Instance.diveCurve.Evaluate(progressRotating);
    }

    IEnumerator WaitAndInvertSensDive()
    {
        yield return new WaitForSeconds(0.25f);
        rotatingSens = -1f;
        inverting = false;
    }


    void FixedUpdate()
    {
        float depth = SwimGameManager.Instance.swimmingArea.transform.position.y - transform.position.y;

        bool inAir = depth<=0f;

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


    public void SetData(string id, Vector2 coordinates)
    {
        data = new SwimmerData(id);
        data.coordinates = coordinates;
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
        pos.y = SwimGameManager.Instance.swimmingArea.transform.position.y+heightDropFrom;
        transform.position = pos;
        rb.linearVelocity = Vector3.zero;
    }

    public void SetTargetPos()
    {
        targetPos = SwimGameManager.Instance.GetRemappedPosition(data.coordinates);
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

        float depth = SwimGameManager.Instance.swimmingArea.transform.position.y - transform.position.y;
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

        //float depth = SwimGameManager.Instance.swimmingArea.transform.position.y - transform.position.y;
       // bool isUnderwater = depth > 0f;

       // if (isUnderwater)
       // {
            rb.AddForce(Vector3.up * waterJumpImpulse*0.25f, ForceMode.VelocityChange);
            rb.AddTorque(Vector3.up * forceSwirl, ForceMode.Force);
            isSwirling = true;
       // }
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
    }
}

[System.Serializable]
public class SwimmerData
{
    public string id;
    public Vector2 coordinates;

    public SwimmerData(string id)
    {
        this.id = id;
    }
}