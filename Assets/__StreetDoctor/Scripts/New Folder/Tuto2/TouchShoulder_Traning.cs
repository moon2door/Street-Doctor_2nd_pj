using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchShoulder_Traning : MonoBehaviour
{
    public CPRTraningStart cprTraningStart;
    private bool isOK = true;
    public int step;
    private int nextstep = 0;

    private void Update()
    {
        if (nextstep > 6)
        {
            cprTraningStart.TriggerStep(step);
            isOK = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.tag);

        if (other.CompareTag("Hand"))
        {
            if (isOK)
            {
                nextstep++;
                //Debug.Log("½ÇÇàµÊ");
            }
            else
            {
                return;
            }
        }
    }
}
