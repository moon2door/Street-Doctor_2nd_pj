using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PadAttachTrigger : MonoBehaviourPun
{
    private bool isAttached = false;
    private AEDPoint currentPoint = null;

    void OnTriggerEnter(Collider other)
    {
        if (isAttached) return;

        AEDPoint point = other.GetComponent<AEDPoint>();
        if (point != null)
        {
            currentPoint = point;
        }
    }

    void OnTriggerExit(Collider other)
    {
        AEDPoint point = other.GetComponent<AEDPoint>();
        if (point != null && point == currentPoint)
        {
            currentPoint.ResetColor();
            currentPoint = null;
        }
    }

    void Update()
    {
        if (isAttached || currentPoint == null) return;

        // ✅ 검지 트리거(PrimaryIndexTrigger) 입력 감지 (오른손 기준)
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            AEDPad grab = GetComponent<AEDPad>();
            FeedbackManager feedback = GetComponent<FeedbackManager>();

            if (grab != null)
            {
                bool match = (currentPoint.slotID == grab.padID);
                currentPoint.ShowFeedback(match);

                if (feedback != null)
                    feedback.ShowFeedback(match);

                if (match)
                {
                    photonView.RPC("AttachPad", RpcTarget.AllBuffered,
                        currentPoint.transform.position,
                        currentPoint.transform.rotation,
                        currentPoint.transform.name);
                    isAttached = true;
                }
            }
        }
    }

    [PunRPC]
    void AttachPad(Vector3 pos, Quaternion rot, string parentName)
    {
        transform.position = pos;
        transform.rotation = rot;

        Transform parent = GameObject.Find(parentName)?.transform;
        if (parent != null)
            transform.SetParent(parent);

        isAttached = true;
    }
}