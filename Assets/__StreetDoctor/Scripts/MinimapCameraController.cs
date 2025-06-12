using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraController : MonoBehaviour
{
    public GameObject mapCenter;

    void Start()
    {
        transform.position = new Vector3(mapCenter.transform.position.x, 50f, mapCenter.transform.position.z); // y는 높이
        transform.rotation = Quaternion.Euler(90f, 0, 0); // 위에서 내려다보기
    }
}
