using UnityEngine;

public class HelicopterAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void LandHelicopter()
    {
        animator.SetTrigger("land");
    }
}
