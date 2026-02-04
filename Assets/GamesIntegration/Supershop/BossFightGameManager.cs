using Katpatat.Networking;
using Unity.Collections;
using Katpatat.Networking.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Splines;
using NUnit.Framework;
using UnityEngine.Rendering;
using Unity.VisualScripting;

public class BossFightGameManager : MonoBehaviour
{
    public GameObject[] prefabsObjects;

    public float throwElevationAngle = 45f;
    public Vector2 minMaxThrowForce;
    public static BossFightGameManager Instance;
    public Animator bullAnimator;
    public SplineAnimate splineAnimate;
    public Vector2 minMaxLateralPositionSpawn;
    public GameObject crosshair;

    bool gameStarted = false;
    public Camera cam;
    public float distanceCrosshair;

    [UnityEngine.Range(0f,1f)]
    public float posX;
    [UnityEngine.Range(0f,1f)]
    public float posY;

    public float speedCursor;
    public float p,freqY,freqX,phiX,phiY;
    public Vector2 minMaxCursorX, minMaxCursorY;

    float currentMultSpeed = 1f;
    float targetMultSpeed = 1f;
    float timerChangeSpeed = 1f;
    public Vector2 minMaxMultSpeed;
    public Vector2 minMaxTimeChangeMultSpeed;

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
        targetMultSpeed = speedCursor;
        crosshair.SetActive(false);
        splineAnimate.NormalizedTime = 0.51f;
        splineAnimate.Pause();
    }

    void Update()
    {
        ManageCursorPosition();

        if(Input.GetKeyDown(KeyCode.Space))
            ThrowRandomObjectRandomAngle();
    }

    void ManageCursorPosition()
    {
        timerChangeSpeed-=Time.deltaTime;
        if(timerChangeSpeed<0f)
        {
            timerChangeSpeed = Random.Range(minMaxTimeChangeMultSpeed.x,minMaxTimeChangeMultSpeed.y);
            targetMultSpeed = Random.Range(minMaxMultSpeed.x,minMaxMultSpeed.y);
            if(Random.value>0.9f)
                targetMultSpeed*=-1f;
        }

        currentMultSpeed = Mathf.Lerp(currentMultSpeed,targetMultSpeed,0.05f);

        p+=Time.deltaTime*speedCursor*currentMultSpeed;
        posX = Mathf.Lerp(minMaxCursorX.x,minMaxCursorX.y,0.5f+Mathf.Cos((p+phiX)*Mathf.PI*2f*freqX)*0.5f);
        posY = Mathf.Lerp(minMaxCursorY.x,minMaxCursorY.y,0.5f+Mathf.Cos((p+phiY)*Mathf.PI*2f*freqY)*0.5f);

        SetCursorPosition(posX,posY);
    }

    public void StartGame()
    {
        crosshair.SetActive(true);
        bullAnimator.SetTrigger("Walk");
        splineAnimate.Play();
        gameStarted = true;
    }

    public void EndGame()
    {
        crosshair.SetActive(false);
        splineAnimate.Pause();
        bullAnimator.SetTrigger("Death");
        gameStarted = false;
    }

    void ThrowRandomObjectRandomAngle()
    {
        int duration = Random.Range(10,250);
        int indexObject = Random.Range(0,prefabsObjects.Length);

        float fromX = Random.value;
        float fromY = 1f;
        float toX = Random.value;
        float toY = 0.5f;

        ThrowObjectDataReceived("noID",fromX,fromY,toX,toY,duration,indexObject);
    }

    public void ThrowObjectDataReceived(string id, float fromX, float fromY, float toX, float toY, int duration, int indexObject)
    {
        if(!gameStarted)
            return;

        Vector2 dir = new Vector2(toX-fromX,(1f-toY)-(1f-fromY)).normalized;
        float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

        float d = Mathf.Clamp01((1000f-duration)/1000f);
        float strenght = Mathf.Lerp(minMaxThrowForce.x,minMaxThrowForce.y,d);

        ThrowObject(id,indexObject,strenght,angle);
    }

    void ThrowObject(string idPlayer,int indexObject, float force,float angleOffset)
    {
        GameObject pick = prefabsObjects[indexObject];

        Ray ray = cam.ViewportPointToRay(new Vector3(posX, posY, 0f));
        float spawnDistance = 1.5f;
        Vector3 spawnPos = ray.origin + ray.direction * spawnDistance;

        GameObject projectile = Instantiate(pick,spawnPos,Random.rotation,transform);
        projectile.GetComponent<ObjectThrow>().SetPlayerID(idPlayer);

        angleOffset = Mathf.Clamp(angleOffset,-45f,45f);

        Vector3 rotated = Quaternion.Euler(0f, angleOffset, 0f) * ray.direction;

        projectile.GetComponent<Rigidbody>().AddForce(rotated*force, ForceMode.Impulse);
    }

    public void TriggerHurtAnimation()
    {
        bullAnimator.SetBool("Hurt",true);
    }

    public void SetCursorPosition(float x, float y)
    {
        crosshair.transform.position = NormalizedToWorld(x, y);
    }

    public Vector3 NormalizedToWorld(float x, float y)
    {
        return cam.ViewportToWorldPoint(new Vector3(x, y, distanceCrosshair));
    }

    public void OnPlayerHitOrMiss(string id, bool hit)
    {
        var hitMessage = new NormalMessage {
            packet = "NORMAL",
            header = "boss-player-hit",
            moduleId = NetworkClient.config.serverConfig.targetSupershopModuleId, // Target module
            args = new JArray {
                id,
                hit
            }
        };

        var jsonMessage = JsonConvert.SerializeObject(hitMessage);
        NetworkClient.SendWebSocketMessage(jsonMessage);
    }
}