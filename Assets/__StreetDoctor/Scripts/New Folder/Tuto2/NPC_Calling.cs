using System.Collections;
using UnityEngine;

public class NPC_Calling : MonoBehaviour
{
    public ForwardRayLine forward;
    public CPRTraningStart cprTraningStart;

    private bool hasStarted = false;

    void Update()
    {
        if (hasStarted) return;

        if (forward.isBlink && forward.currentTarget != null)
        {
            hasStarted = true;
            StartCoroutine(HandleSingleNPC(forward.currentTarget));
        }
    }

    IEnumerator HandleSingleNPC(NPC_OKSign targetNPC)
    {
        targetNPC.PlayPhoneAnim(); // 정확히 가리킨 NPC만
        yield return new WaitForSeconds(1f);

        if (cprTraningStart != null)
        {
            cprTraningStart.TriggerStep(7);

                if (targetNPC != null)
                targetNPC.gameObject.SetActive(false);
        }
        else
        {
            yield break;
        }
    }
}
