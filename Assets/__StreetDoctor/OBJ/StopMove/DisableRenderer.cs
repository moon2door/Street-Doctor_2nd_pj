using UnityEngine;

public class DisableRenderer : MonoBehaviour
{

    void Start()
    {
        GetComponent<Renderer>().enabled = false;

    }

    void Update()
    {

    }
}
