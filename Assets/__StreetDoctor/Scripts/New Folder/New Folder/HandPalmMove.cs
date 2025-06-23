using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class HandPalmMove : MonoBehaviour
{
    [Header("참조")]
    public Transform leftHand;
    public Transform cameraRig;
    public OVRHand ovrHand;          // ← 손가락 상태 확인용

    public Transform rightHand;
    public OVRHand ovrHandRight;

    [Header("설정")]
    public float moveSpeed = 1.5f;
    public float palmMoveThreshold = 0.5f;

    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        if (cameraRig == null)
            cameraRig = Camera.main.transform;

        leftHand = GameObject.Find("LeftHandAnchor").transform;
        cameraRig = GameObject.Find("CenterEyeAnchor").transform;
        ovrHand = GameObject.Find("OVRLeftHandDataSource").GetComponent<OVRHand>();

        rightHand = GameObject.Find("RightHandAnchor").transform;
        ovrHandRight = GameObject.Find("OVRRightHandDataSource").GetComponent<OVRHand>();

    }

    void Update()
    {
        if (!IsFist()) return;

        float upDotL = Vector3.Dot(leftHand.up, Vector3.up);
        float upDotR = Vector3.Dot(rightHand.up, Vector3.up);

        Vector3 moveDir = Vector3.zero;

        if (upDotL < -0.5f && upDotR < -0.5f)
        {
            // 양 손목이 하늘을 향함 → 뒤로
            moveDir = -leftHand.forward;
        }
        else if (upDotL > 0.5f && upDotR > 0.5f)
        {
            // 양 손목이 바닥을 향함 → 앞으로
            moveDir = leftHand.forward;
        }

        moveDir.y = 0;
        characterController.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
    }

    bool IsFist()
    {
        float threshold = 0.3f;

        // 왼손
        float thumbL = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb);
        float indexL = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
        float middleL = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Middle);

        // 오른손
        float thumbR = ovrHandRight.GetFingerPinchStrength(OVRHand.HandFinger.Thumb);
        float indexR = ovrHandRight.GetFingerPinchStrength(OVRHand.HandFinger.Index);
        float middleR = ovrHandRight.GetFingerPinchStrength(OVRHand.HandFinger.Middle);

        //Debug.Log($"[왼] Thumb:{thumbL:F2}, Index:{indexL:F2}, Middle:{middleL:F2} || [오] Thumb:{thumbR:F2}, Index:{indexR:F2}, Middle:{middleR:F2}");

        return thumbL > threshold && indexL > threshold && middleL > threshold &&
               thumbR > threshold && indexR > threshold && middleR > threshold;
    }


}
