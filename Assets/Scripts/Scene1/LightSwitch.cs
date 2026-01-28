using System;
using SmallHedge.SoundManager;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    [SerializeField] private Animator leftLightAnimator;
    [SerializeField] private Animator rightLightAnimator;
    private bool isLightOn = true;
    
    public void DoLights()
    {
        if (isLightOn)
        {
            SoundManager.PlaySound(SoundType.LIGHTOFF);
        }
        else
        {
            SoundManager.PlaySound(SoundType.LIGHTON);
        }
        leftLightAnimator.SetTrigger("Lights");
        rightLightAnimator.SetTrigger("Lights");
        isLightOn = !isLightOn;
    }
}
