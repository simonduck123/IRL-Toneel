using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

public class SplineSpeedController : MonoBehaviour
{
    public CinemachineSplineDolly cinemachineSplineDolly;
    
    public void SetSpeed(float speed)
    {
        var autodolly = cinemachineSplineDolly.AutomaticDolly.Method as SplineAutoDolly.FixedSpeed;
        if (autodolly != null)
            autodolly.Speed = speed;
    }
}
