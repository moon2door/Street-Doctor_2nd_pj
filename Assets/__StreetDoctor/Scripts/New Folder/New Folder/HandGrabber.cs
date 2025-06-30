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

    public float grabThreshold = 0.5f;

    private readonly string[] fingerPrefixes = { "ThumbTip", "IndexTip" };

    void Start()
    {
        // 오른손
        rightOVRHand = GameObject.Find("OVRRightHandDataSource").GetComponent<OVRHand>();
        rightHandTransform = GameObject.Find("XRHand_IndexTipR").transform;

        if (rightFingers == null || rightFingers.Length == 0)
        {
            rightFingers = new GameObject[fingerPrefixes.Length];
            for (int i = 0; i < fingerPrefixes.Length; i++)
            {
                string name = $"XRHand_{fingerPrefixes[i]}R";
                rightFingers[i] = GameObject.Find(name);
                if (rightFingers[i] == null)
                    Debug.LogWarning($"❌ 오른손 손가락 오브젝트 없음: {name}");
            }
        }

        // → null이든 아니든 무조건 AttachFingerTrigger 실행
        foreach (var finger in rightFingers)
        {
            if (finger != null)
                AttachFingerTrigger(finger, isLeft: false);
        }

        // 왼손
        leftOVRHand = GameObject.Find("OVRLeftHandDataSource").GetComponent<OVRHand>();
        leftHandTransform = GameObject.Find("XRHand_IndexTipL").transform;

        if (leftFingers == null || leftFingers.Length == 0)
        {
            leftFingers = new GameObject[fingerPrefixes.Length];
            for (int i = 0; i < fingerPrefixes.Length; i++)
            {
                string name = $"XRHand_{fingerPrefixes[i]}L";
                leftFingers[i] = GameObject.Find(name);
                if (leftFingers[i] == null)
                    Debug.LogWarning($"❌ 왼손 손가락 오브젝트 없음: {name}");
            }
        }

        // → null이든 아니든 무조건 AttachFingerTrigger 실행
        foreach (var finger in leftFingers)
        {
            if (finger != null)
                AttachFingerTrigger(finger, isLeft: true);
        }
    }



    void Update()
    {
        HandleGrab(leftOVRHand, leftHandTransform, ref leftTarget);
        HandleGrab(rightOVRHand, rightHandTransform, ref rightTarget);
    }

    void HandleGrab(OVRHand ovrHand, Transform handTransform, ref GrabbableObject target)
    {
        float thumb = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb);
        float index = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Index);

        if (ovrHand == null || handTransform == null) return;

        bool isPinching = thumb > grabThreshold && index > grabThreshold;
        bool isGrabbingObject = target != null && target.transform.parent == handTransform;

        // Grab
        if (isPinching && target != null && !isGrabbingObject)
        {
            Debug.Log($" {(handTransform == leftHandTransform ? "왼손" : "오른손")} 물체 잡음: {target.name}");
            target.Grab(handTransform);
        }

        // 고정 위치
        if (isGrabbingObject)
        {
            target.transform.localPosition = target.grabOffset;
            target.transform.localRotation = target.grabRotationOffset;
        }

        // Release
        bool isPinchReleased = thumb < grabThreshold || index < grabThreshold;

        if (isGrabbingObject && isPinchReleased)
        {
            Debug.Log($" {(handTransform == leftHandTransform ? "왼손" : "오른손")} 핀치 해제 → 잡은 물체 놓기");
            target.Release();
            target = null;
        }
    }

    void AttachFingerTrigger(GameObject fingerObj, bool isLeft)
    {
        Debug.Log($"👉 {fingerObj.name} 에 FingerTrigger 붙이는 중");

        if (fingerObj.GetComponent<FingerTrigger>() == null)
        {
            FingerTrigger trigger = fingerObj.AddComponent<FingerTrigger>();
            Debug.Log($"✅ FingerTrigger 추가 완료: {fingerObj.name}");
            trigger.Setup(this, isLeft);
        }
        else
        {
            Debug.Log($"⚠️ 이미 FingerTrigger 있음: {fingerObj.name}");
        }
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
