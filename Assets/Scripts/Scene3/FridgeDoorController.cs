using SmallHedge.SoundManager;
using UnityEngine;

public class FridgeDoorController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool isClosed = true;

    public void DoFridgeAnimation()
    {
        if (isClosed)
        {
            SoundManager.PlaySound(SoundType.FridgeOpen);
        }
        else
        {
            SoundManager.PlaySound(SoundType.FridgeClose);
        }
        animator.SetTrigger("Fridge");
        isClosed = !isClosed;
    }
 }
