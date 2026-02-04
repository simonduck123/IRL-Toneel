using System;
using System.Collections.Generic;
using UnityEngine;

public class IdleNPCManager : MonoBehaviour
{
    public List<GameObject> NPCS = new List<GameObject>();
    
    private void OnEnable()
    {
        GunshotManager.gunshotEvent += KillNPC;
    }

    private void OnDisable()
    {
        GunshotManager.gunshotEvent -= KillNPC;
    }

    public void AddNPC(GameObject npc)
    {
        NPCS.Add(npc);
    }
    
    private void KillNPC()
    {
        if (NPCS.Count == 0)
            return;

        GameObject npc = NPCS[0];

        if (npc == null)
        {
            NPCS.RemoveAt(0);
            return;
        }

        Animator anim = npc.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("die");
        }

        NPCS.RemoveAt(0);
    }

}