using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteCloth : MonoBehaviour
{
    public GameObject cloth;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hand"))
        {
            cloth.SetActive(false);
        }
    }
}
