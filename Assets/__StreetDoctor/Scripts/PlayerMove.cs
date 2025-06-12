using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviourPun
{
    [Header("����")]
    public Transform cameraRigRoot;   // OVRCameraRig

    [Header("�÷��̾� �̵��ӵ�")]
    public float moveSpeed = 2.0f;

    [Header("�÷��̾� ȸ�� �ݰ�")]
    public float turnAngle = 45.0f;   // ȸ�� ����
    public float turnThreshold = 0.8f; // ���̽�ƽ �ΰ��� �Ӱ谪

    private CharacterController characterController;
    private bool canTurn = true; // ȸ�� ��Ÿ�� �����

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

        // ���� ���̽�ƽ �̵�
        Vector2 moveInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

        // �Ӹ� ������ �ƴ϶� cameraRig �������� ���� ����
        Vector3 forward = Vector3.ProjectOnPlane(cameraRigRoot.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cameraRigRoot.right, Vector3.up).normalized;
        Vector3 direction = (forward * moveInput.y + right * moveInput.x);

        // �̵� ����
        characterController.Move(direction * moveSpeed * Time.deltaTime);
        cameraRigRoot.position = transform.position;

        // ������ ���̽�ƽ ȸ�� (���� ��)
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