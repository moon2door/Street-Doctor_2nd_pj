using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPRMain : MonoBehaviour
{
    [Header("CPR Zone 오브젝트")]
    public GameObject startZone;
    public GameObject resetZone;
    public GameObject[] pressureZones; // A~E (위→아래 1cm 간격)

    [Header("상태 변수")]
    private bool isCPRActive = false;
    private bool hasPressedDown = false;
    public float cprCount = 0;

    void Start()
    {
        // 초기 세팅
        startZone.SetActive(true);
        resetZone.SetActive(false);
        SetPressureZones(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CPR_Start"))
        {
            BeginCPR();
        }

        else if (isCPRActive && IsPressureZone(other))
        {
            HandlePressureZoneHit(other.gameObject);
        }

        else if (other.CompareTag("CPR_Re") && hasPressedDown)
        {
            EndCPRCycle();
        }
    }

    // ▶️ Start Zone 진입 처리
    void BeginCPR()
    {
        startZone.SetActive(false);
        isCPRActive = true;
        hasPressedDown = false;
        SetPressureZones(true); // A~E 켜기
        resetZone.SetActive(false);
    }

    // ⬇️ A~E 압박 감지 처리
    void HandlePressureZoneHit(GameObject zone)
    {
        if (zone.activeSelf)
        {
            zone.SetActive(false);
            hasPressedDown = true;
            resetZone.SetActive(true); // A~E 중 하나라도 눌리면 리셋존 활성화
        }
    }

    // ⬆️ ResetZone 진입 시 처리
    void EndCPRCycle()
    {
        // 깊이 체크 (몇 개 꺼졌는지)
        int depth = 0;
        foreach (var zone in pressureZones)
        {
            if (!zone.activeSelf) depth++;
        }

        Debug.Log($"CPR 압박 깊이: {depth}cm");
        TrainingEvaluator.Instance.RecordCPR(depth);

        // 다음 사이클 준비
        cprCount++;
        Debug.Log($"CPR 횟수 증가: {cprCount}");

        SetPressureZones(false); // A~E 꺼주기
        startZone.SetActive(true); // Start 존 다시 활성화
        resetZone.SetActive(false);
        isCPRActive = false;
        hasPressedDown = false;
    }

    // 🔄 A~E 오브젝트 일괄 켜기/끄기
    void SetPressureZones(bool state)
    {
        foreach (var obj in pressureZones)
        {
            obj.SetActive(state);
        }
    }

    // 압박 오브젝트인지 확인
    bool IsPressureZone(Collider other)
    {
        foreach (var obj in pressureZones)
        {
            if (other.gameObject == obj) return true;
        }
        return false;
    }

    // 외부에서 CPR 카운트 확인 가능
    public int GetCPRCount() => (int)cprCount;
}
