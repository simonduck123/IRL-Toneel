using UnityEngine;

public class BrotherAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void BrotherHit()
    {
        animator.SetTrigger("Hook");
    }
    public void BrotherTalk()
    {
        animator.SetTrigger("Talk");
    }
    public void BrotherNod()
    {
        animator.SetTrigger("HeadNod");
    }
    public void BrotherArgue()
    {   
        animator.SetTrigger("Argue");
    }
    public void BrotherLaugh()
    {
        animator.SetTrigger("Laugh");
    }
    public void BrotherHitBody()
    {
        animator.SetTrigger("HitBody");
    }
    public void BrotherJump()
    {
        animator.SetTrigger("Jump");
    }
}
