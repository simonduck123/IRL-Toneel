using System;
using UnityEngine;
using SmallHedge.SoundManager;

public class CastleDoorAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool isOpen;

    private void Start()
    {
        isOpen = false;
    }

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
        if (!isOpen)
        {
            SoundManager.PlaySound(SoundType.DOORSLAM);
        }
        
        animator.SetTrigger("open");
        isOpen = !isOpen;
    }
}
