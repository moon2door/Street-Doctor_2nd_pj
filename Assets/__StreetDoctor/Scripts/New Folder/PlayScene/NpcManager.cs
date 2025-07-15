using System.Collections.Generic;
using UnityEngine;

public class NpcManager : MonoBehaviour
{
    public static NpcManager Instance;

    public List<RandomPatrol> allNPCs = new List<RandomPatrol>();
    private HashSet<RandomPatrol> alreadyReactedNPCs = new HashSet<RandomPatrol>();

    private bool isHitOccurred = false;
    private Vector3 fallenNpcPosition;

    [Header("반응 거리 설정")]
    public float minReactDistance = 3f;
    public float maxReactDistance = 20f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterNPC(RandomPatrol npc)
    {
        if (!allNPCs.Contains(npc))
            allNPCs.Add(npc);
    }

    public bool TryTriggerNPC(RandomPatrol triggeringNPC)
    {
        if (isHitOccurred) return false;

        isHitOccurred = true;
        fallenNpcPosition = triggeringNPC.transform.position;

        foreach (var npc in allNPCs)
        {
            if (npc == triggeringNPC) continue;

            float dist = Vector3.Distance(npc.transform.position, fallenNpcPosition);
            if (dist >= minReactDistance && dist <= maxReactDistance)
            {
                npc.ReactToFallenNPC(fallenNpcPosition);
                alreadyReactedNPCs.Add(npc);
            }
        }

        return true;
    }

    /// 🆕 쓰러진 이후에도 범위에 들어오는 NPC에게 반응시키기
    public void Update()
    {
        if (!isHitOccurred) return;

        foreach (var npc in allNPCs)
        {
            if (alreadyReactedNPCs.Contains(npc)) continue;

            float dist = Vector3.Distance(npc.transform.position, fallenNpcPosition);
            if (dist >= minReactDistance && dist <= maxReactDistance)
            {
                npc.ReactToFallenNPC(fallenNpcPosition);
                alreadyReactedNPCs.Add(npc);
            }
        }
    }

    public bool HasTriggered => isHitOccurred;
}
