using UnityEngine;

public class BrotherAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void BrotherHit()
    {
        animator.SetTrigger("Hook");
    }
}
