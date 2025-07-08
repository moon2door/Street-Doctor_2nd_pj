using System.Collections.Generic;
using UnityEngine;

public class TrainingEvaluator : MonoBehaviour
{
    public static TrainingEvaluator Instance;

    [Header("Check 항목")]
    public bool didCheckConscious = false;
    public bool didCallHelp = false;
    public bool padsPlacedCorrectly = false;

    private List<string> aedSequence = new List<string>();

    [Header("CPR 시간 기록")]
    private float cprStartTime = -1f; // CPR 시작 기준 시간
    private List<float> cprTimestamps = new List<float>(); // CPR 수행 시간 (상대 기준)

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // ✅ CPR 수행 시 시간 기록 (CPR 시작 이후 상대시간으로)
    public void RecordCPRTimestamp()
    {
        if (cprStartTime < 0)
            cprStartTime = Time.time;

        float relativeTime = Time.time - cprStartTime;
        cprTimestamps.Add(relativeTime);
    }

    // ✅ AED 버튼 순서 기록
    public void AddAEDButton(string btnName)
    {
        aedSequence.Add(btnName);
    }

    // ✅ 패드 부착 여부
    public void SetPadsPlaced(bool placed)
    {
        padsPlacedCorrectly = placed;
    }

    // ✅ 최종 피드백 출력
    public void PrintFeedback()
    {
        Debug.Log("====== 훈련 피드백 ======");

        LogCheck("어깨를 두드려 의식 확인을", didCheckConscious);

        LogCheck("주변 사람에게 AED 요청 및 119 신고 지시를", didCallHelp);

        EvaluateCPR();

        bool aedCorrect = IsSequenceCorrect(new List<string> { "BtnOpen", "BtnR", "BtnShock" });
        LogCheck("AED 버튼을 올바른 순서대로 누르기", aedCorrect);

        LogCheck("패드를 올바른 위치에 부착", padsPlacedCorrectly);

        Debug.Log("====== 피드백 종료 ======");
    }


    // ✅ AED 버튼 순서 평가
    private bool IsSequenceCorrect(List<string> correct)
    {
        if (aedSequence.Count < correct.Count) return false;
        for (int i = 0; i < correct.Count; i++)
        {
            if (aedSequence[i] != correct[i])
                return false;
        }
        return true;
    }

    // ✅ CPR 속도 평가 (CPR 시작 기준 상대 시간 기준)
    private void EvaluateCPR()
    {
        if (cprTimestamps.Count == 0)
        {
            Debug.Log("[X] CPR을 수행하지 않았습니다.");
            return;
        }

        float duration = cprTimestamps[cprTimestamps.Count - 1]; // CPR 수행 시간 (시작 후)
        int totalCount = cprTimestamps.Count;
        float rate = totalCount / duration;

        string result = rate >= 1.7f ? " [V] " : " [X] ";

        Debug.Log($"{result} CPR 속도: 총 {totalCount}회 / {duration:F1}초 → {rate:F2}회/초");
    }

    private void LogCheck(string title, bool condition)
    {
        string mark = condition ? "[V]" : "[X]";
        string result = condition ? $"{title} 하였습니다." : $"{title} 하지 않았습니다.";
        Debug.Log($"{mark} {result}");
    }

}
