using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomController : MonoBehaviour
{
    public Animator animator;
    private string currentAnimaton;

    const string MOM_IDLE = "Mom_Idle";
    const string MOM_TALK = "Mom_Talking";
    const string MOM_STANDTALK = "Mom_StandingTalk";
    const string MOM_TALKPHONE = "Mom_TalkPhone";
    const string MOM_TEXTWALK = "Mom_TextWalk";
    const string MOM_WALK = "Mom_Walk";

    public void Idle()
    {
        ChangeAnimationState(MOM_IDLE);
    }

    public void Walk()
    {
        ChangeAnimationState(MOM_WALK);
    }

    public void Talk()
    {
        ChangeAnimationState(MOM_TALK);
    }

    public void StandTalk()
    {
        ChangeAnimationState(MOM_STANDTALK);
    }

    public void TalkPhone()
    {
        ChangeAnimationState(MOM_TALKPHONE);
    }

    public void TextWalk()
    {
        ChangeAnimationState(MOM_TEXTWALK);
    }

    void ChangeAnimationState(string newAnimation)
    {
        if (currentAnimaton == newAnimation) return;

        animator.Play(newAnimation);
        currentAnimaton = newAnimation;
    }
}
