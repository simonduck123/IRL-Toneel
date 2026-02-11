using System;
using System.Collections.Generic;
using SmallHedge.SoundManager;
using UnityEngine;


public class CasinoNPCManager : MonoBehaviour
{
    public List<GameObject> NPCS = new List<GameObject>();
    private bool isKillable = false;
    

    private void OnEnable()
    {
        GunshotManager.gunshotEvent += KillNPC;
    }

    private void OnDisable()
    {
        GunshotManager.gunshotEvent -= KillNPC;
    }


    private void KillNPC()
    {
        if (!isKillable)
            return;
        
        if (NPCS.Count == 0)
            return;

        GameObject npc = NPCS[0];

        if (npc == null)
        {
            NPCS.RemoveAt(0);
            return;
        }

        Animator anim = npc.GetComponent<Animator>();
        npc.GetComponent<CopMover>()?.SetIsDead();

        if (anim != null)
        {
            anim.SetTrigger("die");
        }

        NPCS.RemoveAt(0);
    }

    public void SetIsKillable(bool isKillable)
    {
        this.isKillable = isKillable;
    }
}
