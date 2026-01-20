using UnityEngine;
using UnityEngine.VFX;

public class ObjectThrow : MonoBehaviour
{
    public VisualEffect particles;
    bool hasParticles = false;
    bool alreadyTriggered = false;
    public Animator bullAnimator;
    void Start()
    {
        hasParticles = particles!=null;
    }

    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if(!hasParticles)
            return;

        if(alreadyTriggered)
            return;

        if(collision.collider.gameObject.layer != LayerMask.NameToLayer("Bull"))
            return;

        particles.Play();
        alreadyTriggered = true;
        BossFightGameManager.Instance.TriggerHurtAnimation();
    }
}
