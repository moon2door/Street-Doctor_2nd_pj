using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    public Transform handTransform;
    private GameObject grabbed;

    void Update()
    {
        if (grabbed == null && OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
        {
            Collider[] hits = Physics.OverlapSphere(handTransform.position, 0.1f);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("AEDPad"))
                {
                    grabbed = hit.gameObject;
                    grabbed.transform.SetParent(handTransform);
                    grabbed.transform.localPosition = Vector3.zero;
                    grabbed.transform.localRotation = Quaternion.identity;
                    break;
                }
            }
        }

        if (grabbed != null && OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
        {
            grabbed.transform.SetParent(null);
            grabbed = null;
        }
    }
}