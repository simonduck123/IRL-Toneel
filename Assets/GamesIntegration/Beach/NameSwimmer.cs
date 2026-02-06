using UnityEngine;
using TMPro;

public class NameSwimmer : MonoBehaviour
{
    public TMP_Text nameArea;
    public Swimmer swimmer;
    public float distanceCamera = 0.11f;

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
    }

    void ManagePosition()
    {
        if(swimmer==null)
            return;

        if(SwimGameManager.Instance.cameraFrom==null)
            return;

        if(SwimGameManager.Instance.cameraTo==null)
            return;
            
        Vector3 screenPos = SwimGameManager.Instance.cameraFrom.WorldToScreenPoint(swimmer.transform.position);
        screenPos.z = distanceCamera;
        Vector3 worldPos = SwimGameManager.Instance.cameraTo.ScreenToWorldPoint(screenPos);
        transform.position = worldPos;
        transform.rotation = SwimGameManager.Instance.cameraTo.transform.rotation;
    }
}
