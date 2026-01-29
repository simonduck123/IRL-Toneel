using System;
using SmallHedge.SoundManager;
using UnityEngine;
using UnityEngine.Events;
public class GunshotManager : MonoBehaviour
{
    public static event Action gunshotEvent;
    
    public void PlayAKGunshot()
    {
        SoundManager.PlaySound(SoundType.AKSHOT);
        gunshotEvent?.Invoke();
    }
    
    public void PlayAKReload()
    {
        SoundManager.PlaySound(SoundType.AKRELOAD);
    }
    
    public void PlayUziGunshot()
    {
        SoundManager.PlaySound(SoundType.UZISHOT);
        gunshotEvent?.Invoke();
    }

    public void PlayUziReload()
    {
        SoundManager.PlaySound(SoundType.UZIRELOAD);
    }
}