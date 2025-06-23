using UnityEngine;

public class HandGrabber : MonoBehaviour
{
    [Header("Hand Reference")]
    public OVRHand rightOVRHand;
    public OVRHand leftOVRHand;
    public GameObject[] rightFingers;
    public GameObject[] leftFingers;
    private GrabbableObject rightTarget;
    private GrabbableObject leftTarget;

    [Header("손 Transform")]
    public Transform rightHandTransform;
    public Transform leftHandTransform;

    public float grabThreshold = 0.9f;

    private readonly string[] fingerPrefixes = { "ThumbTip", "IndexTip" };

    void Start()
    {
        // 오른손
        rightOVRHand = GameObject.Find("OVRRightHandDataSource").GetComponent<OVRHand>();
        rightHandTransform = GameObject.Find("XRHand_IndexTipR").transform;

        rightFingers = new GameObject[fingerPrefixes.Length];
        for (int i = 0; i < fingerPrefixes.Length; i++)
        {
            string name = $"XRHand_{fingerPrefixes[i]}R";
            rightFingers[i] = GameObject.Find(name);
            if (rightFingers[i] != null)
                AttachFingerTrigger(rightFingers[i], isLeft: false);
            else
                Debug.LogWarning($"❌ 오른손 손가락 오브젝트 없음: {name}");
        }

        // 왼손
        leftOVRHand = GameObject.Find("OVRLeftHandDataSource").GetComponent<OVRHand>();
        leftHandTransform = GameObject.Find("XRHand_IndexTipL").transform;

        leftFingers = new GameObject[fingerPrefixes.Length];
        for (int i = 0; i < fingerPrefixes.Length; i++)
        {
            string name = $"XRHand_{fingerPrefixes[i]}L";
            leftFingers[i] = GameObject.Find(name);
            if (leftFingers[i] != null)
                AttachFingerTrigger(leftFingers[i], isLeft: true);
            else
                Debug.LogWarning($"❌ 왼손 손가락 오브젝트 없음: {name}");
        }
    }

    void Update()
    {
        HandleGrab(leftOVRHand, leftHandTransform, ref leftTarget);
        HandleGrab(rightOVRHand, rightHandTransform, ref rightTarget);
    }

    void HandleGrab(OVRHand ovrHand, Transform handTransform, ref GrabbableObject target)
    {
        if (ovrHand == null || handTransform == null) return;

        float thumb = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb);
        float index = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Index);

        bool isPinching = thumb > grabThreshold && index > grabThreshold;
        bool isGrabbingObject = target != null && target.transform.parent == handTransform;

        // Grab
        if (isPinching && target != null && !isGrabbingObject)
        {
            target.Grab(handTransform);
        }

        // 고정 위치
        if (isGrabbingObject)
        {
            target.transform.localPosition = Vector3.zero;
            target.transform.localRotation = Quaternion.identity;
        }

        // Release
        bool isPinchReleased = thumb < grabThreshold || index < grabThreshold;

        if (isGrabbingObject && isPinchReleased)
        {
            Debug.Log($"🔓 {(handTransform == leftHandTransform ? "왼손" : "오른손")} 핀치 해제 → 잡은 물체 놓기");
            target.Release();
            target = null;
        }
    }

    void AttachFingerTrigger(GameObject fingerObj, bool isLeft)
    {
        FingerTrigger trigger = fingerObj.AddComponent<FingerTrigger>();
        trigger.Setup(this, isLeft);
    }

    public void SetTarget(GrabbableObject obj, bool isLeft)
    {
        if (isLeft) leftTarget = obj;
        else rightTarget = obj;
    }

    public void ClearTarget(GrabbableObject obj)
    {
        if (rightTarget == obj) rightTarget = null;
        if (leftTarget == obj) leftTarget = null;
    }
}
