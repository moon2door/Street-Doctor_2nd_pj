using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneDisable : MonoBehaviour
{
    public bool disable = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    IEnumerator phone()
    {
        if (disable) yield break;

        yield return new WaitForSeconds(0.5f);
        this.gameObject.SetActive(false);
        disable = true;
    }
}
