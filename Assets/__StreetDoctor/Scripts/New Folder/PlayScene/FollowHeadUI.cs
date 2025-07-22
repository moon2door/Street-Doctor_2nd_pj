using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHeadUI : MonoBehaviour
{
    public Transform playerHead; // ∫∏≈Î¿∫ Camera.main.transform
    public Vector3 offset = new Vector3(0, 0, 2f);
    public float followSpeed = 5f;

    void Update()
    {
        Vector3 targetPosition = playerHead.position + playerHead.forward * offset.z + playerHead.up * offset.y;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

        Quaternion targetRotation = Quaternion.LookRotation(transform.position - playerHead.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * followSpeed);
    }
}

