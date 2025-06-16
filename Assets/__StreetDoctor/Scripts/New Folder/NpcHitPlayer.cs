using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcHitPlayer : MonoBehaviour
{
    public RandomPatrol patrol;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            patrol.StopAndRotate90();
        }
    }
}
