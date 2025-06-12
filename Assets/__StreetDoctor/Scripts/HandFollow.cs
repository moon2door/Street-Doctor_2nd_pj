using UnityEngine;
using Photon.Pun;

public class HandFollow : MonoBehaviourPun
{
    public string anchorName; // "LeftHandAnchor" ¶Ç´Â "RightHandAnchor"
    private Transform anchor;

    void Start()
    {
        if (!photonView.IsMine)
        {
            enabled = false;
            return;
        }

        GameObject anchorObj = GameObject.Find(anchorName);
        if (anchorObj != null)
            anchor = anchorObj.transform;
    }

    void Update()
    {
        if (anchor != null)
        {
            transform.position = anchor.position;
            transform.rotation = anchor.rotation;
        }
    }
}
