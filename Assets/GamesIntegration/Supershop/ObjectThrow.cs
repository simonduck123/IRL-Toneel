using UnityEngine;
using UnityEngine.VFX;

public class ObjectThrow : MonoBehaviour
{
    public VisualEffect particles;
    bool hasParticles = false;
    bool alreadyTriggered = false;
    public Animator bullAnimator;
    public string idPlayer;
    public float timeAlive;
    
    void Start()
    {
        hasParticles = particles!=null;
    }

    void Update()
    {
        timeAlive+=Time.deltaTime;
        if(timeAlive>2f && !alreadyTriggered)
        {
            alreadyTriggered = true;
            BossFightGameManager.Instance.OnPlayerHitOrMiss(idPlayer,false);
        }
    }

    public void SetPlayerID(string id)
    {
        idPlayer = id;
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
        BossFightGameManager.Instance.OnPlayerHitOrMiss(idPlayer,true);
    }
}
