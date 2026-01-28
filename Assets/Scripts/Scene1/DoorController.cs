using SmallHedge.SoundManager;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void KnockTwo()
    {
        SoundManager.PlaySound(SoundType.KNOCK2);
        animator.SetTrigger("two");
    }
    
    public void KnockThree()
    {
        SoundManager.PlaySound(SoundType.KNOCK3);
        animator.SetTrigger("three");
    }
    
    public void DoorDo()
    {
        animator.SetTrigger("open");
    }
}
