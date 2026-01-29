using SmallHedge.SoundManager;
using UnityEngine;

public class ParticleActivator : MonoBehaviour
{
    public ParticleSystem effect;
    public AudioSource source;

    public void PlayEffect()
    {
        if (effect != null)
        {
            effect.Play();
            PlayGunshot();
        }
    }
    
    public void PlayGunshot()
    {
        //SoundManager.PlaySound(SoundType.GUNSHOT);
    }
}
