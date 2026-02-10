using UnityEngine;

public class RatMover : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private Animator animator;

    private Transform endPoint;
    private bool hasReachedEnd;
    private bool isDead = false;
    private bool isIdle = true;

    public void SetEndPoint(Transform target)
    {
        endPoint = target;
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }

        if (isIdle)
        {
            return;
        }
        
        if (hasReachedEnd || endPoint == null)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            endPoint.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, endPoint.position) < 0.01f)
        {
            hasReachedEnd = true;
            transform.position = endPoint.position;
            
        }
    }

    public void SetIsDead()
    {
        isDead = true;
    }
    
    public void SetIsIdle()
    {
        isIdle = false;
    }
}