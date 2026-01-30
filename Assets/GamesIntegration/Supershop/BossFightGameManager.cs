using Unity.Collections;
using Katpatat.Networking.Utils;
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
    public bool invertVertical = true;
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
        p+=Time.deltaTime*speedCursor;
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

        ThrowObjectDataReceived("noID",duration,indexObject);
    }

    public void ThrowObjectDataReceived(string id, int duration, int indexObject)
    {
        if(!gameStarted)
            return;


        float d = Mathf.Clamp01((1000f-duration)/1000f);
        float strenght = Mathf.Lerp(minMaxThrowForce.x,minMaxThrowForce.y,d);

        ThrowObject(id,indexObject,strenght);
    }

    void ThrowObject(string idPlayer,int indexObject, float force)
    {
        GameObject pick = prefabsObjects[indexObject];

        Ray ray = cam.ViewportPointToRay(new Vector3(posX, posY, 0f));
        float spawnDistance = 1.5f;
        Vector3 spawnPos = ray.origin + ray.direction * spawnDistance;

        GameObject projectile = Instantiate(pick,spawnPos,Random.rotation,transform);
        projectile.GetComponent<ObjectThrow>().SetPlayerID(idPlayer);
        projectile.GetComponent<Rigidbody>().AddForce(ray.direction*force, ForceMode.Impulse);
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
        Katpatat.Networking.NetworkClient.SendWebSocketMessage(JsonUtility.ToJson(new SupershopPlayerHit(id, hit)));
    }
}

[System.Serializable]
public class SupershopPlayerHit
{
    public string header = "boss-player-hit";
    public string id;
    public bool hit;
    public SupershopPlayerHit(string id, bool hit)
    {
        this.id = id;
        this.hit = hit;
    } 
}