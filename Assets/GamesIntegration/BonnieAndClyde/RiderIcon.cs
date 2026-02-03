using UnityEngine;

public class RiderIcon : MonoBehaviour
{
    public Rider rider;
    public float distanceCamera = 0.11f;

    public void SetRider(Rider r)
    {
        rider = r;
    }
    void Update()
    {
        ManagePosition();
    }

    void ManagePosition()
    {
        if(rider==null)
            return;

        if(RiderGameManager.Instance.cam==null)
            return;
            
        Vector3 screenPos = RiderGameManager.Instance.cam.WorldToScreenPoint(rider.transform.position);
        screenPos.z = distanceCamera;
        Vector3 worldPos = RiderGameManager.Instance.cam.ScreenToWorldPoint(screenPos);
        transform.position = worldPos;
        transform.rotation = RiderGameManager.Instance.cam.transform.rotation;
    }
}
