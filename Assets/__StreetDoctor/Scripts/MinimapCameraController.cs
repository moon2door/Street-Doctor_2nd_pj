using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraController : MonoBehaviour
{
    public GameObject mapCenter;

    void Start()
    {
        transform.position = new Vector3(mapCenter.transform.position.x, 50f, mapCenter.transform.position.z); // y�� ����
        transform.rotation = Quaternion.Euler(90f, 0, 0); // ������ �����ٺ���
    }
}
