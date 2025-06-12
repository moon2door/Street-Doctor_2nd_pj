using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviourPun
{
    [Header("참조")]
    public Transform cameraRigRoot;   // OVRCameraRig

    [Header("플레이어 이동속도")]
    public float moveSpeed = 2.0f;

    [Header("플레이어 회전 반경")]
    public float turnAngle = 45.0f;   // 회전 각도
    public float turnThreshold = 0.8f; // 조이스틱 민감도 임계값

    private CharacterController characterController;
    private bool canTurn = true; // 회전 쿨타임 제어용

    void Start()
    {
        if (!photonView.IsMine)
        {
            enabled = false;
            return;
        }

        characterController = GetComponent<CharacterController>();

        if (cameraRigRoot == null)
            cameraRigRoot = GameObject.Find("OVRCameraRig")?.transform;
    }

    void Update()
    {
        if (cameraRigRoot == null) return;

        // 왼쪽 조이스틱 이동
        Vector2 moveInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

        // 머리 기준이 아니라 cameraRig 기준으로 방향 설정
        Vector3 forward = Vector3.ProjectOnPlane(cameraRigRoot.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cameraRigRoot.right, Vector3.up).normalized;
        Vector3 direction = (forward * moveInput.y + right * moveInput.x);

        // 이동 적용
        characterController.Move(direction * moveSpeed * Time.deltaTime);
        cameraRigRoot.position = transform.position;

        // 오른쪽 조이스틱 회전 (스냅 턴)
        Vector2 turnInput = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        if (canTurn)
        {
            if (turnInput.x > turnThreshold)
            {
                RotateYaw(turnAngle);
                StartCoroutine(TurnCooldown());
            }
            else if (turnInput.x < -turnThreshold)
            {
                RotateYaw(-turnAngle);
                StartCoroutine(TurnCooldown());
            }
        }
    }

    void RotateYaw(float angle)
    {
        cameraRigRoot.Rotate(Vector3.up, angle);
    }

    System.Collections.IEnumerator TurnCooldown()
    {
        canTurn = false;
        yield return new WaitUntil(() => Mathf.Abs(OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x) < 0.2f);
        canTurn = true;
    }
}