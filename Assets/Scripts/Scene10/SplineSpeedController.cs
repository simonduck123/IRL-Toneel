using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

public class SplineSpeedController : MonoBehaviour
{
    public CinemachineSplineDolly cinemachineSplineDolly;
    [SerializeField] private ParticleSystem kissParticle;
    
    public void SetSpeed(float speed)
    {
        var autodolly = cinemachineSplineDolly.AutomaticDolly.Method as SplineAutoDolly.FixedSpeed;
        if (autodolly != null)
            autodolly.Speed = speed;
    }

    
    public void PlayKiss()
    {
        kissParticle.Play();
    }
}
