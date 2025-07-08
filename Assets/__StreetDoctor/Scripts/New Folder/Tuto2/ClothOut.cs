using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothOut : MonoBehaviour
{
    public Animator myAnim;
    public CPRTraningStart cprTS;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            myAnim.SetTrigger("Cloth_Touch");

            StartCoroutine(Cloth_Delay());
        }
    }

    IEnumerator Cloth_Delay()
    {
        yield return new WaitForSeconds(1.2f);

        this.gameObject.SetActive(false);
        cprTS.TriggerStep(30);
    }
}
