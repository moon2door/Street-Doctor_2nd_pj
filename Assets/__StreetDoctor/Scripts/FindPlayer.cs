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

    [Tooltip("플레이어 높이 보정")]
    public float headp = 1.6f;

    public Transform camHead;   // CenterEyeAnchor
    public Transform camLeft;   // LeftHandAnchor
    public Transform camRight;  // RightHandAnchor

    private Vector3 lastCamPos;
    GameObject eyeObj;
    GameObject lhObj;
    GameObject rhObj;

    private float initialCamHeadY; // 고정 기준 높이

    void Start()
    {
        //if (!photonView.IsMine)
        //{
        //    enabled = false;
        //    return;
        //}
        //else
        //{
        //    Debug.Log("포톤없음");
        //}

        if (camHead == null)
        {
            eyeObj = GameObject.Find("CenterEyeAnchor");
            lhObj = GameObject.Find("LeftHandAnchor");
            rhObj = GameObject.Find("RightHandAnchor");
        }

        if (eyeObj != null)
        {
            camHead = eyeObj.transform;
            lastCamPos = camHead.position;
            initialCamHeadY = camHead.position.y; // 기준 높이 저장
        }

        if (lhObj != null) camLeft = lhObj.transform;
        if (rhObj != null) camRight = rhObj.transform;
    }


    void Update()
    {
        if (camHead == null) return;

        // 카메라 이동량 계산 (Y는 따로 headp로 보정하므로 제외)
        Vector3 delta = camHead.position - lastCamPos;
        transform.position += new Vector3(delta.x, 0f, delta.z);

        // Y축은 headp 보정값으로 직접 세팅
        Vector3 newPos = transform.position;
        newPos.y = initialCamHeadY - headp;
        transform.position = newPos;

        lastCamPos = camHead.position;

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

        // 몸통은 제자리에서 회전만 하도록 처리
        if (modelbody && camHead)
        {
            // 머리 방향의 Y축만 따라가게
            Vector3 forward = Vector3.ProjectOnPlane(camHead.forward, Vector3.up).normalized;
            if (forward.sqrMagnitude > 0.001f)
                modelbody.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }
    }
}
