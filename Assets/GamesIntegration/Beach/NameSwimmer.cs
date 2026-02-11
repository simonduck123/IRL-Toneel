using UnityEngine;
using TMPro;

public class NameSwimmer : MonoBehaviour
{
    public TMP_Text nameArea;
    public Swimmer swimmer;
    public float distanceCamera = 0.11f;
    float baseAlphaName = 1f;

    void Start()
    {
        baseAlphaName = nameArea.color.a;
    }

    public void SetSwimmer(Swimmer s)
    {
        swimmer = s;

        string[] separated = s.data.nickname.Split(" ");

        if(separated.Length!=2)
            separated = new[]{"",""};

        string final = separated[0]+"\n"+separated[1];
        nameArea.text = final;
    }

    public void ShowName(bool show)
    {
        nameArea.enabled = show;
    }

    void Update()
    {
        ManagePosition();

        if(SwimGameManager.Instance.namesFading)
        {
            float alphaName = Mathf.Lerp(baseAlphaName,0f,Mathf.Clamp01(SwimGameManager.Instance.timeNameFading/30f));
            Color c = nameArea.color;
            c.a = alphaName;
            nameArea.color = c;
        }

    }

    void ManagePosition()
    {
        if(swimmer==null)
            return;

        if(SwimGameManager.Instance.cam==null)
            return;
  
        Vector3 screenPos = SwimGameManager.Instance.cam.WorldToScreenPoint(swimmer.transform.position);
        screenPos.z = distanceCamera;
        Vector3 worldPos = SwimGameManager.Instance.cam.ScreenToWorldPoint(screenPos);
        transform.position = worldPos;
        transform.rotation = SwimGameManager.Instance.cam.transform.rotation;
    }
}
