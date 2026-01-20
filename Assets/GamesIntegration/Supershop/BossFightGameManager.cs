using Unity.Collections;
using Katpatat.Networking.Utils;
using UnityEngine;

public class BossFightGameManager : MonoBehaviour
{
    public GameObject[] prefabsObjects;

    public float throwElevationAngle = 45f;
    public Vector2 minMaxThrowForce;
    public static BossFightGameManager Instance;
    public GameObject throwReference;

    private void OnEnable() 
    {
        NetworkMessageUtil.OnThrowObject += ThrowObjectDataReceived;
    }
    private void OnDisable() 
    {
        NetworkMessageUtil.OnThrowObject -= ThrowObjectDataReceived;
    }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            ThrowRandomObjectRandomAngle();
    }

    void ThrowRandomObjectRandomAngle()
    {
        float fromX = Random.value;
        float fromY = Random.Range(0f,0.4f);

        float toX = Random.value;
        float toY = Random.Range(0.5f,1f);

        int duration = Random.Range(10,250);
        int indexObject = Random.Range(0,prefabsObjects.Length);

        ThrowObjectDataReceived("noID",fromX,fromY,toX,toY,duration,indexObject);
    }

    public void ThrowObjectDataReceived(string id, float fromX, float fromY, float toX, float toY, int duration, int indexObject)
    {
        Vector2 from = new Vector2(fromX,fromY);
        Vector2 to = new Vector2(toX,toY);
        Vector2 direction = to-from;
        float magnitude = direction.magnitude;
        direction = direction.normalized;

        //use length or duration to calculate the strenght of the throw
        bool useLength = true;
        float d = 0.5f;

        if(useLength)
            d = magnitude/Mathf.Sqrt(2f);
        else
            d = (1000f-duration)/1000f;

        d = Mathf.Clamp01(d);

        float strenght = Mathf.Lerp(minMaxThrowForce.x,minMaxThrowForce.y,d);

        Vector3 directionLaunch = DirectionLaunch(direction);
        ThrowObject(fromX,directionLaunch,strenght);
    }

    Vector3 DirectionLaunch(Vector2 direction)
    {
        Vector3 worldDir = throwReference.transform.forward * direction.y + throwReference.transform.right * direction.x;
        Quaternion xRot = Quaternion.AngleAxis(throwElevationAngle, Vector3.Cross(worldDir, Vector3.up));
        worldDir = xRot * worldDir;

        return worldDir.normalized;
    }

    void ThrowObject(float lateralPosition, Vector3 direction, float force)
    {
        Vector3 pos = throwReference.transform.position-throwReference.transform.forward+throwReference.transform.right*Mathf.Lerp(-2f,2f,lateralPosition);
        GameObject pick = prefabsObjects[Random.Range(0,prefabsObjects.Length)];
        GameObject projectile = Instantiate(pick,pos,Random.rotation,throwReference.transform);
        projectile.GetComponent<Rigidbody>().AddForce(direction*force, ForceMode.Impulse);
    }
}