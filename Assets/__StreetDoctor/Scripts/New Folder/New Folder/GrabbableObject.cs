using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    [HideInInspector] public bool isGrabbed = false;

    [Header("릴리즈 시 붙을 부모")]
    public Transform returnParent;

    public void Grab(Transform handTransform)
    {
        isGrabbed = true;
        transform.SetParent(handTransform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Release()
    {
        isGrabbed = false;
        if (returnParent != null)
        {
            transform.SetParent(returnParent);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            transform.SetParent(null);
        }
    }

}
