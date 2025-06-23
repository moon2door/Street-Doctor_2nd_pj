using UnityEngine;

public class LeftHandFistDetector : MonoBehaviour
{
    public OVRHand ovrHandLeft;

    void Start()
    {
        if (ovrHandLeft == null)
        {
            GameObject leftHandObj = GameObject.Find("OVRLeftHandDataSource");
            if (leftHandObj != null)
                ovrHandLeft = leftHandObj.GetComponent<OVRHand>();
        }
    }

    public bool IsLeftHandFist()
    {
        if (ovrHandLeft == null) return false;

        float t = ovrHandLeft.GetFingerPinchStrength(OVRHand.HandFinger.Thumb);
        float i = ovrHandLeft.GetFingerPinchStrength(OVRHand.HandFinger.Index);
        float m = ovrHandLeft.GetFingerPinchStrength(OVRHand.HandFinger.Middle);

        return t > 0.5f && i > 0.5f && m > 0.5f;
    }
}
