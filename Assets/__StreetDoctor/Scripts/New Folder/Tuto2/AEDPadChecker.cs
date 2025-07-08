using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AEDPadChecker : MonoBehaviour
{
    public GrabbableObject pad1;
    public GrabbableObject pad2;

    public CPRTraningStart cprTS;

    private bool triggered = false;

    void Update()
    {
        //Debug.LogError("1번 패드 상태 : " + pad1.isOK_CPR);
        //Debug.LogError("2번 패드 상태 : " + pad2.isOK_CPR);

        if (!triggered && pad1.isOK_CPR && pad2.isOK_CPR)
        {
            triggered = true;
            if (cprTS != null)
            {
                cprTS.TriggerStep(34);
            }
            else
            {
                TrainingEvaluator.Instance.SetPadsPlaced(true);
            }
            
            //Debug.LogError("34번체크 실행됨");
        }
    }

}
