using UnityEngine;

public class VaultController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void VaultDo()
    {
        animator.SetTrigger("do");
    }
}
