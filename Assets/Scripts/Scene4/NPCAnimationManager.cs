using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCAnimationManager : MonoBehaviour
{
    [Header("Animation")]
    public Animator animator;
    public List<AnimationClip> animations = new List<AnimationClip>();

    [Tooltip("Fade duration between animations")]
    public float crossFadeTime = 0.25f;
    
    private int lastIndex = -1;

    void Start()
    {
        if (animator != null && animations.Count > 0)
        {
            StartCoroutine(PlayRandomAnimations());
        }
    }

    IEnumerator PlayRandomAnimations()
    {
        while (true)
        {
            int index;
            do
            {
                index = Random.Range(0, animations.Count);
            }
            while (index == lastIndex && animations.Count > 1);

            lastIndex = index;

            AnimationClip clip = animations[index];

            animator.CrossFade(clip.name, crossFadeTime);
            
            yield return new WaitForSeconds(
                Mathf.Max(0f, clip.length - crossFadeTime)
            );
        }
    }
}