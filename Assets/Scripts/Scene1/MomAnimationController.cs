using UnityEngine;

public class MomAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void MomPlayIdleAnim()
    {
        animator.SetTrigger("Idle");
    }
    public void MomPlayStrongArgueAnim()
    {
        animator.SetTrigger("StrongArgue");
    }
    public void MomPlayArgueAnim()
    {
        animator.SetTrigger("Argue");
    }
    public void MomPlayTalkAnim()
    {
        animator.SetTrigger("Talk");
    }
    public void MomPlayTalkAngryAnim()
    {
        animator.SetTrigger("TalkAngry");
    }
    public void MomPlayLaughAnim()
    {
        animator.SetTrigger("Laugh");
    }
    public void MomPlayCallAnim()
    {
        animator.SetTrigger("Call");
    }
    
}
