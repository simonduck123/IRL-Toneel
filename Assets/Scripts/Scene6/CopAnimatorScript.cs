using UnityEngine;

public class CopAnimatorScript : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void CopFall()
    {
        animator.SetTrigger("Fall");
    }
}
