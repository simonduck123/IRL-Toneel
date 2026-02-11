using System;
using System.Collections.Generic;
using SmallHedge.SoundManager;
using UnityEngine;

public class RatManager : MonoBehaviour
{
    public List<GameObject> NPCS = new List<GameObject>();
    public List<GameObject> NPCOptions = new List<GameObject>();
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private Transform copEndPoint;
    [SerializeField] private SoundType soundType;

    private void OnEnable()
    {
        GunshotManager.gunshotEvent += KillNPC;
    }

    private void OnDisable()
    {
        GunshotManager.gunshotEvent -= KillNPC;
    }

    public void AddNPC()
    {
        GameObject npcPrefab = NPCOptions[UnityEngine.Random.Range(0, NPCOptions.Count)];
        GameObject newNPC = Instantiate(
            npcPrefab,
            spawnPoint.transform.position,
            spawnPoint.transform.rotation
        );

        NPCS.Add(newNPC);
        
        SoundManager.PlaySound(soundType);
        
        RatMover moverRat = newNPC.GetComponent<RatMover>();
        if (moverRat != null)
        {
            moverRat.SetEndPoint(copEndPoint);
        }
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
        npc.GetComponent<RatMover>().SetIsDead();
        if (anim != null)
        {
            anim.SetTrigger("die");
        }

        NPCS.RemoveAt(0);
    }

}