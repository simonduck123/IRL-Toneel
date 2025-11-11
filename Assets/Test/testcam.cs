using UnityEngine;

public class testcam : MonoBehaviour
{
    public RenderTexture sourceTexture;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // Replace screen output with your RenderTexture
        Graphics.Blit(sourceTexture, dest);
    }
}
