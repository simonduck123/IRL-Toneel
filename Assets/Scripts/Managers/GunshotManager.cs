using SmallHedge.SoundManager;
using UnityEngine;

public class GunshotManager : MonoBehaviour
{
    public void PlayAKGunshot()
    {
        SoundManager.PlaySound(SoundType.AKSHOT);
    }
    
    public void PlayAKReload()
    {
        SoundManager.PlaySound(SoundType.AKRELOAD);
    }
    
    public void PlayUziGunshot()
    {
        SoundManager.PlaySound(SoundType.UZISHOT);
    }

    public void PlayUziReload()
    {
        SoundManager.PlaySound(SoundType.UZIRELOAD);
    }
}