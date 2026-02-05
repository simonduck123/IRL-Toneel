using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

public class SplineSpeedController : MonoBehaviour
{
    [SerializeField] private float dollySpeed = 0.015f;
    public CinemachineSplineDolly cinemachineSplineDolly;
    
    public void SetSpeed()
    {
        var autodolly = cinemachineSplineDolly.AutomaticDolly.Method as SplineAutoDolly.FixedSpeed;
        if (autodolly != null)
            autodolly.Speed = dollySpeed;
    }
}
