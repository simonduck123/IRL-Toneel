using System;
using System.Collections.Generic;
using SmallHedge.SoundManager;
using UnityEngine;


public class CasinoNPCManager : MonoBehaviour
{
    public List<GameObject> NPCS = new List<GameObject>();
    public List<GameObject> NPCOptions = new List<GameObject>();

    [SerializeField] private SpawnData[] spawnPoints; // paired spawn + end
    [SerializeField] private SoundType soundType;

    private int currentSpawnIndex = 0;
    private SpawnData activeSpawnData;

    private void Awake()
    {
        if (spawnPoints.Length > 0)
            activeSpawnData = spawnPoints[0];
    }

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
        if (activeSpawnData == null || NPCOptions.Count == 0)
            return;

        GameObject npcPrefab = NPCOptions[UnityEngine.Random.Range(0, NPCOptions.Count)];

        GameObject newNPC = Instantiate(
            npcPrefab,
            activeSpawnData.spawnPoint.position,
            activeSpawnData.spawnPoint.rotation
        );

        NPCS.Add(newNPC);

        //SoundManager.PlaySound(soundType);

        CopMover mover = newNPC.GetComponent<CopMover>();
        if (mover != null)
        {
            mover.SetEndPoint(activeSpawnData.endPoint);
        }
    }

    public void SetActiveSpawnPoint()
    {
        if (spawnPoints.Length == 0)
            return;
        
        currentSpawnIndex = (currentSpawnIndex + 1) % spawnPoints.Length;

        activeSpawnData = spawnPoints[currentSpawnIndex];
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
        npc.GetComponent<CopMover>()?.SetIsDead();

        if (anim != null)
        {
            anim.SetTrigger("die");
        }

        NPCS.RemoveAt(0);
    }
}
