using Oculus.Interaction.Body.Input;
using Photon.Pun;
using UnityEngine;

public class FindPlayer : MonoBehaviourPun
{
    [Header("모델 A 안의 머리, 손 본")]
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

        // 하이라키에 있는 OVR 카메라 리그에서 Anchor들 찾기
        GameObject eyeObj = GameObject.Find("CenterEyeAnchor");
        GameObject lhObj = GameObject.Find("LeftHandAnchor");
        GameObject rhObj = GameObject.Find("RightHandAnchor");

        if (eyeObj != null) camHead = eyeObj.transform;
        if (lhObj != null) camLeft = lhObj.transform;
        if (rhObj != null) camRight = rhObj.transform;
    }

    void Update()
    {
        // 플레이어 전체 위치를 카메라 위치로 이동 (Y만 headp 만큼 내림)
        transform.position = new Vector3(camHead.position.x, camHead.position.y - headp, camHead.position.z);

        // 머리, 손 위치/회전 복사
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

        // 몸통 위치를 머리 기준으로 하고, 회전은 Y축만 따라가게
        if (modelbody && camHead)
        {
            // 머리 위치 기준으로 Y만 -1.57f 내린 위치에 바디 배치
            Vector3 bodyPos = camHead.position + Vector3.down * 1.1f;
            modelbody.position = bodyPos;

            // Y축 회전만 따르도록
            Vector3 forward = Vector3.ProjectOnPlane(camHead.forward, Vector3.up).normalized;
            if (forward.sqrMagnitude > 0.001f)
                modelbody.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }
    }
}
