using UnityEngine;

public class DadAnimatioinController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void DadLaugh()
    {
        animator.SetTrigger("laugh");
    }
    public void DadIdle()
    {
        animator.SetTrigger("idle");
    }
    public void DadGolf()
    {
        animator.SetTrigger("golf");
    }
    public void DadGuitar()
    {   
        animator.SetTrigger("guitar");
    }
    public void DadSalute()
    {   
        animator.SetTrigger("salute");
    }
    public void DadClapping()
    {   
        animator.SetTrigger("clap");
    }
}
