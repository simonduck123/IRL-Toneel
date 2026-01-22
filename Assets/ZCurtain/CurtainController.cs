using UnityEngine;

public class CurtainController : MonoBehaviour
{
    public Animator animator;
    public void DoAnimation()
    {
        animator.SetTrigger("do");
    }
}
