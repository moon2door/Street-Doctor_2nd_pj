using UnityEngine;
using System;

public class ClapDetector : MonoBehaviour
{
    [Header("손 참조")]
    public Transform leftHand;
    public Transform rightHand;
    public GameObject right_IndexTip;         // 여기에 라인랜더러가 붙어 있음

    [Header("감지 설정")]
    public float clapDistanceThreshold = 0.15f;
    public float approachSpeedThreshold = 1.0f;
    public float cooldownTime = 0.5f;

    public Action OnClapDetected;

    private float previousDistance;
    private float lastClapTime = -999f;

    private LineRenderer indexTipLine;        // ← LineRenderer 캐싱용

    void Start()
    {
        leftHand = GameObject.Find("XRHand_PalmL")?.transform;
        rightHand = GameObject.Find("XRHand_PalmR")?.transform;
        right_IndexTip = GameObject.Find("XRHand_IndexTipR");

        if (leftHand == null || rightHand == null || right_IndexTip == null)
        {
            Debug.LogWarning("ClapDetector: 손 오브젝트를 제대로 찾지 못했습니다.");
            return;
        }

        indexTipLine = right_IndexTip.GetComponent<LineRenderer>();
        if (indexTipLine == null)
        {
            Debug.LogWarning("ClapDetector: XRHand_IndexTip 오브젝트에 LineRenderer가 없습니다.");
        }

        previousDistance = GetHandDistance();
    }

    void Update()
    {
        DetectClap();
    }

    void DetectClap()
    {
        float currentDistance = GetHandDistance();
        float deltaDistance = previousDistance - currentDistance;
        float deltaSpeed = deltaDistance / Time.deltaTime;

        bool closeEnough = currentDistance < clapDistanceThreshold;
        bool fastEnough = deltaSpeed > approachSpeedThreshold;
        bool cooledDown = Time.time - lastClapTime > cooldownTime;

        if (closeEnough && fastEnough && cooledDown)
        {
            lastClapTime = Time.time;
            Debug.Log("👏 박수 감지됨!");

            ToggleIndexLine();
            OnClapDetected?.Invoke();
        }

        previousDistance = currentDistance;
    }

    float GetHandDistance()
    {
        if (leftHand == null || rightHand == null) return float.MaxValue;
        return Vector3.Distance(leftHand.position, rightHand.position);
    }

    void ToggleIndexLine()
    {
        if (indexTipLine == null) return;
        indexTipLine.enabled = !indexTipLine.enabled;
    }
}
