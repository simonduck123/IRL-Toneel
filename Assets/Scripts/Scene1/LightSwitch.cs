using System;
using SmallHedge.SoundManager;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    [SerializeField] private Animator leftLightAnimator;
    [SerializeField] private Animator rightLightAnimator;
    
    public void DoLights()
    {
        leftLightAnimator.SetTrigger("Lights");
        rightLightAnimator.SetTrigger("Lights");
        SoundManager.PlaySound(SoundType.LIGHT);
    }
}
