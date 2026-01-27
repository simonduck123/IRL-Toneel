using UnityEngine;

public class FridgeDoorController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void DoFridgeAnimation()
    {
        animator.SetTrigger("Fridge");
    }
 }
