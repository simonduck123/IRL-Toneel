using SmallHedge.SoundManager;
using UnityEngine;

public class ExplosionAudioSound : MonoBehaviour
{
    public void PlayExplosion()
    {
        SoundManager.PlaySound(SoundType.EXPLOSION);
    }
}
