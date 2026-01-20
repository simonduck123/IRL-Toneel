using UnityEngine;

public class Swimmer : MonoBehaviour
{
    public SwimmerData data;
    public Shark shark;
    
    public bool drowning;

    private float rand;
    private bool isJumping;
    private float jumpProgress;

    private float sin;
    private float timeSinceStarted;

    private bool isFalling;
    private float fallProgress;
    private float fallFrom = 10f;

    private float posY;
    private float heightDropFrom = 80f;
    
    private bool isDying;
    private float dyingProgress;
    private bool startedDrawning = false;

    
    private Vector3 targetPos;
    private Vector3 motion = Vector3.zero;
    public Animator animControl;

    private void Start()
    {
        rand = Random.Range(-1f,1f);
    }

    private void Update()
    {
        Vector3 direction = new Vector3(targetPos.x,0f,targetPos.z)-new Vector3(transform.position.x,0f,transform.position.z);
        float dist = direction.magnitude;
        direction = direction.normalized;

        motion += direction * (Helper.RemapClamp(dist,0.1f,3f,0f,1f) * Random.Range(0.75f,1.5f));
        motion *= 0.95f;
        if(motion.magnitude>50f)
            motion = motion.normalized*50f;

        if(motion.magnitude<1f)
            motion = motion.normalized*1f;

        animControl.SetBool("Swimming",motion.magnitude>5f);


        Vector3 pos = transform.position+motion*Time.deltaTime;

        Vector3 orientation = (pos-transform.position).normalized;

        if(!isJumping && !isFalling)
        {
            timeSinceStarted+= Time.deltaTime;
            sin = Mathf.Sin((rand+timeSinceStarted)*Mathf.PI*2f)*0.5f;
        }
            
        pos.y = SwimGameManager.Instance.swimmingArea.transform.position.y+sin;

        if(isJumping)
        {
            jumpProgress+=Time.deltaTime;

            pos.y+=SwimGameManager.Instance.jumpCurve.Evaluate(Mathf.Clamp01(jumpProgress))*5f;

            if(jumpProgress>1f)
            {
                isJumping = false;
                jumpProgress=0f;
            }
        }

        if(isFalling)
        {
            fallProgress+=Time.deltaTime;

            pos.y+=SwimGameManager.Instance.fallCurve.Evaluate(Mathf.Clamp01(fallProgress))*fallFrom;

            if(fallProgress>1f)
            {
                isFalling = false;
                fallProgress=0f;
            }
        }

        if(isDying && drowning)
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
        transform.position = pos;

        transform.LookAt(transform.position+orientation,Vector3.up); 
    }

    public void Jump()
    {
        if(isDying)
            return;

        if(isFalling)
            return;

        if(isJumping)
            return;

        jumpProgress = -0.01f;
        isJumping = true;
    }

    public void Fall()
    {
        if(isFalling)
            return;

        fallFrom = SwimGameManager.Instance.swimmingArea.transform.position.y+heightDropFrom+Random.Range(-5f,5f);

        posY = fallFrom;
        transform.position = new Vector3(transform.position.x,posY,transform.position.z);

        fallProgress = Random.Range(-0.5f,-0.01f);
        isFalling = true;
    }

    public void Die()
    {
        if(isDying)
            return;

        SwimGameManager.Instance.SpawnShark(this);
        isDying=true;
    }


    public void SetData(string id, Vector2 coordinates)
    {
        data = new SwimmerData(id);
        data.coordinates = coordinates;
    }

    public void SetCoordinates(Vector2 newCoordinates)
    {
        if(isDying)
            return;

        data.coordinates = newCoordinates;
        SetTargetPos();
        if(data.coordinates.x>1f || data.coordinates.x<0f || data.coordinates.y>1f || data.coordinates.y<0f)
            Die();
    }

    public void TeleportAboveCurrentCoordinates()
    {
        SetTargetPos();
        Vector2 randCircle = Random.insideUnitCircle*Random.Range(0.5f,1.5f);

        Vector3 pos = targetPos+new Vector3(randCircle.x,0f,randCircle.y);
        pos.y = SwimGameManager.Instance.swimmingArea.transform.position.y+heightDropFrom;
        posY = pos.y;
        transform.position = pos;
        Fall();
    }

    public void SetTargetPos()
    {
        targetPos = SwimGameManager.Instance.GetRemappedPosition(data.coordinates);
    }
    
    public void HandleAction(string actionName) {
        switch (actionName) {
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
