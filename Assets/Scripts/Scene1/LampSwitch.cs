using SmallHedge.SoundManager;
using UnityEngine;

public class LampSwitch : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool isLightOn = true;
    
    public void DoLampSwitch()
    {
        if (isLightOn)
        {
            SoundManager.PlaySound(SoundType.LIGHTON);
        }
        else
        {
            SoundManager.PlaySound(SoundType.LIGHTOFF);
        }
        
        
        animator.SetTrigger("Lamp");
    }
}
