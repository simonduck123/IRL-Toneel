//"Low Poly Shark" (https://skfb.ly/6GNN6) by AlienDev is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).

using System.Collections;
using UnityEngine;

public class Shark : MonoBehaviour
{
    public Swimmer targetSwimmer;
    bool approaching = true;
    public GameObject boneJaw;
    float progress = 0f;
    Vector3 from;

    void Start()
    {
        from = transform.position;
        OpenMouth(true);
        transform.localEulerAngles = Vector3.up*(Random.value>0.5f?180f:0f);
        StartCoroutine(WaitAndCloseMouth());
    }

    void Update()
    {
        if(targetSwimmer==null)
            return;

        if(approaching)
        {
            progress+=Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position,targetSwimmer.transform.position,progress*progress);

            if(progress>=1f)
            {
                approaching = false;
                transform.position = targetSwimmer.transform.position;
            }
        }
        else
        {
            transform.position = targetSwimmer.transform.position;
        }
    }

    IEnumerator WaitAndCloseMouth()
    {
        yield return new WaitForSeconds(0.35f);
        OpenMouth(false);
    }

    public void OpenMouth(bool open)
    {
        if(!open)
            boneJaw.transform.localPosition = new Vector3(0f,0.005f,0f);
        else
            boneJaw.transform.localPosition = new Vector3(0f,0.01f,0f);
    }

    public void SetTarget(Swimmer target)
    {
        targetSwimmer = target;
        target.shark = this;
    }
}
