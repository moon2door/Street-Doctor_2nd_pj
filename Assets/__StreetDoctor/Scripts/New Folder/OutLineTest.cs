using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutLineTest : MonoBehaviour
{
    Outline myOL;

    void Start()
    {
        myOL = GetComponent<Outline>();
        GameObject.Find("RightHandAnchor").GetComponent<Controller>().outline.AddListener(outline);
    }

    public void outline(string _string)
    {
        if(_string == gameObject.name)
        {
            myOL.enabled = true;
        }
        else
        {
            myOL.enabled = false;
        }
    }
}
