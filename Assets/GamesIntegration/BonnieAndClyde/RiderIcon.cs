using UnityEngine;

public class RiderIcon : MonoBehaviour
{
    public Rider rider;
    public float distanceCamera = 0.11f;
    public Renderer renderer;

    public void SetRider(Rider r)
    {
        rider = r;
        renderer.material.SetColor("_Color",Color.HSVToRGB(Random.value,Random.Range(0f,0.5f),1f));
    }
    void Update()
    {
        ManagePosition();
    }

    void ManagePosition()
    {
        if(rider==null)
            return;

        if(RiderGameManager.Instance.cameraFrom==null)
            return;

        if(RiderGameManager.Instance.cameraTo==null)
            return;
            
        Vector3 screenPos = RiderGameManager.Instance.cameraFrom.WorldToScreenPoint(rider.transform.position);
        screenPos.z = distanceCamera;
        Vector3 worldPos = RiderGameManager.Instance.cameraTo.ScreenToWorldPoint(screenPos);
        transform.position = worldPos;
        transform.rotation = RiderGameManager.Instance.cameraTo.transform.rotation;
    }
}
