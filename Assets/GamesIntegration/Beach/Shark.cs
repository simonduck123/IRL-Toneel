using UnityEngine;

public class Shark : MonoBehaviour
{
    public Swimmer targetSwimmer;
    bool approaching = true;
    void Start()
    {
        
    }

    void Update()
    {
        if(targetSwimmer==null)
            return;

        if(approaching)
        {
            transform.position = Vector3.Lerp(transform.position,targetSwimmer.transform.position,0.25f);
            if(Vector3.Distance(transform.position,targetSwimmer.transform.position)<0.05f)
            {
                approaching = false;
                transform.position = targetSwimmer.transform.position;
                targetSwimmer.drowning = true;
            }
        }
        else
        {
            transform.position = targetSwimmer.transform.position;
        }

    }

    public void SetTarget(Swimmer target)
    {
        targetSwimmer = target;
        target.shark = this;
    }
}
