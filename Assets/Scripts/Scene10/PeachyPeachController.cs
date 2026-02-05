using UnityEngine;

public class PeachyPeachController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void PeachIdle()
    {
        animator.SetTrigger("idle");
    }
    public void PeachKiss()
    {
        animator.SetTrigger("kiss");
    }
    public void PeachTalk()
    {
        animator.SetTrigger("talk");
    }
    public void PeachLaugh()
    {   
        animator.SetTrigger("laugh");
    }
}
