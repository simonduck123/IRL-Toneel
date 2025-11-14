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
            source.Play();
        }
    }
}
