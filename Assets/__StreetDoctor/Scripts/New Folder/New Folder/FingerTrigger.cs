using UnityEngine;

public class FingerTrigger : MonoBehaviour
{
    private HandGrabber grabber;
    private bool isLeftHand;

    public void Setup(HandGrabber grabber, bool isLeft)
    {
        this.grabber = grabber;
        this.isLeftHand = isLeft;
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("손가락이 닿은 물체: " + other.name);

        if (other.CompareTag("Grabbable"))
        {
            GrabbableObject obj = other.GetComponent<GrabbableObject>();
            if (obj != null)
                grabber.SetTarget(obj, isLeftHand);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Grabbable"))
        {
            GrabbableObject obj = other.GetComponent<GrabbableObject>();
            if (obj != null)
            {
                // 현재 손이 잡고 있는 상태면 Clear 방지
                Transform currentHand = isLeftHand ? grabber.leftHandTransform : grabber.rightHandTransform;

                if (obj.transform.parent != currentHand)
                {
                    grabber.ClearTarget(obj);
                }
            }
        }
    }
}
