using Oculus.Interaction.Body.Input;
using Photon.Pun;
using UnityEngine;

public class FindPlayer : MonoBehaviourPun
{
    [Header("�� A ���� �Ӹ�, �� ��")]
    public Transform modelHead;
    public Transform modelLeftHand;
    public Transform modelRightHand;
    public Transform modelbody;

    public float headp;

    private Transform camHead;   // CenterEyeAnchor
    private Transform camLeft;  // LeftHandAnchor
    private Transform camRight; // RightHandAnchor

    void Start()
    {
        if (!photonView.IsMine)
        {
            enabled = false;
            return;
        }

        // ���̶�Ű�� �ִ� OVR ī�޶� ���׿��� Anchor�� ã��
        GameObject eyeObj = GameObject.Find("CenterEyeAnchor");
        GameObject lhObj = GameObject.Find("LeftHandAnchor");
        GameObject rhObj = GameObject.Find("RightHandAnchor");

        if (eyeObj != null) camHead = eyeObj.transform;
        if (lhObj != null) camLeft = lhObj.transform;
        if (rhObj != null) camRight = rhObj.transform;
    }

    void Update()
    {
        // �÷��̾� ��ü ��ġ�� ī�޶� ��ġ�� �̵� (Y�� headp ��ŭ ����)
        transform.position = new Vector3(camHead.position.x, camHead.position.y - headp, camHead.position.z);

        // �Ӹ�, �� ��ġ/ȸ�� ����
        if (camHead && modelHead)
        {
            modelHead.SetParent(camHead);
            modelHead.localPosition = Vector3.zero;
            modelHead.localRotation = Quaternion.identity;
        }

        if (camLeft && modelLeftHand)
        {
            modelLeftHand.SetParent(camLeft);
            modelLeftHand.localPosition = Vector3.zero;
            modelLeftHand.localRotation = Quaternion.identity;
        }

        if (camRight && modelRightHand)
        {
            modelRightHand.SetParent(camRight);
            modelRightHand.localPosition = Vector3.zero;
            modelRightHand.localRotation = Quaternion.identity;
        }

        // ���� ��ġ�� �Ӹ� �������� �ϰ�, ȸ���� Y�ุ ���󰡰�
        if (modelbody && camHead)
        {
            // �Ӹ� ��ġ �������� Y�� -1.57f ���� ��ġ�� �ٵ� ��ġ
            Vector3 bodyPos = camHead.position + Vector3.down * 1.1f;
            modelbody.position = bodyPos;

            // Y�� ȸ���� ��������
            Vector3 forward = Vector3.ProjectOnPlane(camHead.forward, Vector3.up).normalized;
            if (forward.sqrMagnitude > 0.001f)
                modelbody.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }
    }
}
