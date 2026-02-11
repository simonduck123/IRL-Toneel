using System;
using System.Collections.Generic;
using SmallHedge.SoundManager;
using UnityEngine;

[System.Serializable]
public class SpawnData
{
    public Transform spawnPoint;
    public Transform endPoint;
}



public class NPCManager : MonoBehaviour
{
    public List<GameObject> NPCS = new List<GameObject>();
    public List<GameObject> NPCOptions = new List<GameObject>();
    [SerializeField] private SpawnData[] spawnPoints;
    [SerializeField] private SoundType soundType;

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
        if (spawnPoints.Length == 0 || NPCOptions.Count == 0)
            return;

        // Pick random spawn data
        activeSpawnData = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

        GameObject npcPrefab = NPCOptions[UnityEngine.Random.Range(0, NPCOptions.Count)];

        GameObject newNPC = Instantiate(
            npcPrefab,
            activeSpawnData.spawnPoint.position,
            activeSpawnData.spawnPoint.rotation
        );

        NPCS.Add(newNPC);

        SoundManager.PlaySound(soundType);

        CopMover mover = newNPC.GetComponent<CopMover>();
        if (mover != null)
        {
            mover.SetEndPoint(activeSpawnData.endPoint);
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
        npc.GetComponent<CopMover>().SetIsDead();
        if (anim != null)
        {
            anim.SetTrigger("die");
        }

        NPCS.RemoveAt(0);
    }

    public void AddSpawnPoint()
    {
        
    }

}