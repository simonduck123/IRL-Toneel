using UnityEngine;

public class CameraScene3Controller : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    public void DoZoomAninmation()
    {
        animator.SetTrigger("Zoom");
    }
}
