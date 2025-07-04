using System.Collections;
using UnityEngine;

public class NPC_Calling : MonoBehaviour
{
    [Header("모든 NPC_OKSign을 연결하세요")]
    public NPC_OKSign[] npcList;
    public CPRTraningStart cprTraningStart;

    private bool hasStarted = false;

    void Update()
    {
        if (hasStarted) return;

        foreach (var npc in npcList)
        {
            if (npc != null && npc.isBlinking)
            {
                hasStarted = true;
                StartCoroutine(DeactivateAllNPCsAfterDelay(3f));
                break;
            }
        }
    }

    IEnumerator DeactivateAllNPCsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        cprTraningStart.TriggerStep(7);

        foreach (var npc in npcList)
        {
            if (npc != null)
                npc.gameObject.SetActive(false);
        }
    }
}
