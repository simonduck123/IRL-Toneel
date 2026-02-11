using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Assertions.Must;

public class RiderIcon : MonoBehaviour
{
    public Rider rider;
    public float distanceCamera = 0.11f;
    public Renderer renderer;
    public TMP_Text nameArea;
    public float refSize;
    public float refFOV;

    bool iconsAreHidden = false;

    public void SetRider(Rider r)
    {
        rider = r;
        
       string[] separated = rider.nickname.Split(" ");

       if(separated.Length!=2)
            separated = new[]{"",""};

        string final = separated[0]+"\n"+separated[1];
        nameArea.text = final;
    }

    void Start()
    {
         transform.SetParent(RiderGameManager.Instance.camIcons.transform, false);
    }
    void LateUpdate()
    {
        ManagePosition();
    }

    void ManagePosition()
    {
        if(rider==null)
            return;

        if(RiderGameManager.Instance.camIcons==null)
            return;

         Camera cam = RiderGameManager.Instance.camIcons;

        Vector3 viewportPos = cam.WorldToViewportPoint(rider.headPosition.position);

        bool visible =
            viewportPos.z > 0f &&
            viewportPos.x >= 0f && viewportPos.x <= 1f &&
            viewportPos.y >= 0f && viewportPos.y <= 1f;

        SetVisible(visible);

        if (!visible)
            return;

        Vector3 worldPos = cam.ViewportToWorldPoint(new Vector3(viewportPos.x, viewportPos.y, distanceCamera));

        transform.position = worldPos;
        transform.rotation = cam.transform.rotation;

        KeepConstantScreenSize(transform, cam, refFOV, distanceCamera, refSize);
    }

    void KeepConstantScreenSize(Transform obj, Camera cam, float referenceFov, float referenceDistance, float referenceScale)
    {
        float currentDistance = Vector3.Distance(cam.transform.position, obj.position);
        float fovRatio = Mathf.Tan(referenceFov * 0.5f * Mathf.Deg2Rad) /
                        Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);

        float scale = referenceScale * (currentDistance / referenceDistance) * fovRatio;
        obj.localScale = Vector3.one * scale;
    }

    public void SetVisible(bool visible)
    {
        renderer.enabled = visible && !iconsAreHidden;
        nameArea.enabled = visible;
    }

    public void ShowIcon(bool show)
    {
        renderer.enabled = show;
        iconsAreHidden = !show;
    }

    public void SetTextureIcon(Texture2D tex)
    {
        renderer.material.SetTexture("_MainTex",tex);
    }
}

