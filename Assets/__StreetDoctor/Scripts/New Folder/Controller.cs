using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Controller : MonoBehaviour
{
    public UnityEvent<string> outline;
    RaycastHit hit;

    public GameObject rightHand;

    void Update()
    {
        Ray ray = new Ray(rightHand.transform.position, rightHand.transform.forward);

        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red);

        if (Physics.Raycast(ray, out hit, 10f))
        {
            outline.Invoke(hit.collider.gameObject.name);
        }
        else
        {
            outline.Invoke(null);
        }
    }
}
