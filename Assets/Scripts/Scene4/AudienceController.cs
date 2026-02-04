using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudienceController : MonoBehaviour
{
    public List<GameObject> dancers = new List<GameObject>();

    public float slowSpeed = 0.1f;
    public float transitionTime = 10f;

    public void SetSlowMode()
    {
        StartCoroutine(SlowDownAnimators());
    }

    private IEnumerator SlowDownAnimators()
    {
        float elapsed = 0f;

        List<Animator> animators = new List<Animator>();
        foreach (GameObject d in dancers)
        {
            animators.Add(d.GetComponent<Animator>());
        }

        while (elapsed < transitionTime)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / transitionTime;
            float currentSpeed = Mathf.Lerp(1f, slowSpeed, t);
            
            foreach (Animator anim in animators)
            {
                anim.speed = currentSpeed;
            }

            yield return null;
        }

        foreach (Animator anim in animators)
        {
            anim.speed = slowSpeed;
        }
    }

}