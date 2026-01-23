using UnityEngine;

public class Swimmer : MonoBehaviour
{
    public SwimmerData data;
    public Shark shark;
    public Rigidbody rb;
    
    public bool drowning;

    private float rand;

    bool isJumping = false;

    private float posY;
    private float heightDropFrom = 80f;
    
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
    public float airDrag = 0.1f;
    public float waterJumpImpulse = 8f;

    public float maxAcceleration = 30f;
    public float maxSpeed = 6f;
    public float slowRadius = 2f;

    private void Start()
    {
        rand = Random.Range(-1f,1f);
    }

    private void Update()
    {
        /*if(isDying && drowning)
        {
            dyingProgress=Mathf.Clamp01(dyingProgress+Time.deltaTime);
            pos.y+=SwimGameManager.Instance.deathCurve.Evaluate(Mathf.Clamp01(dyingProgress))*-10f;

            if(Mathf.Approximately(dyingProgress, 1f))
            {
                isDying = false;
                drowning = false;
                gameObject.SetActive(false);
                shark.gameObject.SetActive(false);
                if(shark)
                {
                    Destroy(shark.gameObject);
                    shark = null;
                }
                dyingProgress=0f;
                motion = Vector2.zero;
                SetCoordinates(SwimGameManager.Instance.RandomCoordinates());
                

                TeleportAboveCurrentCoordinates();
                pos = transform.position;
                gameObject.SetActive(true);
            }
        }

        posY = Mathf.Lerp(posY,pos.y,0.75f);
        pos.y = posY;
        //transform.position = pos;

        rigidBody.AddForce(motion);

        transform.LookAt(transform.position+orientation,Vector3.up); */
    }

    void FixedUpdate()
    {
        float depth = SwimGameManager.Instance.swimmingArea.transform.position.y - transform.position.y;

        if(depth<0f || isDying)
        {
            rb.linearDamping = airDrag;
            rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);   
        }
        else
        {
            rb.linearDamping = waterDrag;
            float buoyancyForce = buoyancy * Mathf.Clamp01(depth);
            rb.AddForce(Vector3.up * buoyancyForce, ForceMode.Acceleration);
            isJumping = false;

            Vector3 toTarget = targetPos - rb.position;
            float distance = toTarget.magnitude;

            if (distance > 0.01f)
            {
                Vector3 desiredVelocity;

                if (distance > slowRadius)
                {
                    desiredVelocity = toTarget.normalized * maxSpeed;
                }
                else
                {
                    float t = distance / slowRadius;
                    desiredVelocity = toTarget.normalized * (maxSpeed * t);
                }

                Vector3 steering = desiredVelocity - rb.linearVelocity;

                Vector3 acceleration = Vector3.ClampMagnitude(steering, maxAcceleration);

                rb.AddForce(acceleration, ForceMode.Acceleration);
            }

            Vector3 direction = new Vector3(rb.linearVelocity.x,0f,rb.linearVelocity.z).normalized;
            if(rb.linearVelocity.magnitude>0.1f)
                transform.LookAt(transform.position+direction,Vector3.up);
        }

        if(rb.linearVelocity.magnitude>300f)
            rb.linearVelocity = 300f*rb.linearVelocity.normalized;
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
            case "swimDive":
            default:
                Debug.LogWarning($"No action defined for: {actionName}");
                break;
        }
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