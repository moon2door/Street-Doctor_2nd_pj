using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TrainingEvaluator : MonoBehaviour
{
    public static TrainingEvaluator Instance;
    public CarResponder carResponder;

    [Header("Check 항목")]
    public bool didCheckConscious = false;
    public bool didCallHelp = false;
    public bool padsPlacedCorrectly = false;

    public Transform playerTransform;

    private List<string> aedSequence = new List<string>();

    [Header("CPR 시간 및 깊이 기록")]
    private float cprStartTime = -1f; // CPR 시작 기준 시간
    private List<float> cprTimestamps = new List<float>(); // CPR 수행 시간
    private List<int> cprDepths = new List<int>(); // CPR 깊이 (단위: cm)

    [Header("UI 출력")]
    public Text feedbackText;
    public GameObject uiOBJ;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ✅ 씬 전환 시 오브젝트 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (playerTransform == null)
        {
            GameObject target = GameObject.Find("cpr_obj_onoff");

            if (target != null && target.activeInHierarchy) // 존재하고 활성화된 경우에만
            {
                playerTransform = target.transform;
            }
        }
    }

    // ✅ CPR 수행 시 시간 + 깊이(cm) 기록
    public void RecordCPR(int depthCm)
    {
        if (cprStartTime < 0)
            cprStartTime = Time.time;

        float relativeTime = Time.time - cprStartTime;
        cprTimestamps.Add(relativeTime);
        cprDepths.Add(depthCm);
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
        uiOBJ.SetActive(true);

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("====== 훈련 피드백 ======");

        sb.AppendLine($"어깨를 두드려서 의식을 확인했나요? {(didCheckConscious ? "[V]" : "[X]")}");
        sb.AppendLine($"주변 사람을 가리켜 지시를 했나요? {(didCallHelp ? "[V]" : "[X]")}");

        if (cprTimestamps.Count == 0)
        {
            sb.AppendLine("CPR을 수행했나요? [X]");
        }
        else
        {
            sb.AppendLine("CPR을 수행했나요? [V]");
            sb.AppendLine(GetCPRFeedback()); // CPR 평가 결과 텍스트로 반환
        }

        bool aedCorrect = IsSequenceCorrect(new List<string> { "BtnOpen", "BtnR", "BtnShock" });
        sb.AppendLine($"AED 버튼을 순서대로 눌렀나요? {(aedCorrect ? "[V]" : "[X]")}");
        sb.AppendLine($"패드를 올바른 위치에 부착했나요? {(padsPlacedCorrectly ? "[V]" : "[X]")}");
        sb.AppendLine("====== 피드백 종료 ======");

        if (feedbackText != null)
            feedbackText.text = sb.ToString();

        carResponder.ActivateResponse(playerTransform);
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

    /*
    // ✅ CPR 속도 + 깊이 평가
    private void EvaluateCPR()
    {
        if (cprTimestamps.Count == 0)
        {
            Debug.Log("[X] CPR을 수행하지 않았습니다.");
            return;
        }

        float duration = cprTimestamps[cprTimestamps.Count - 1]; // CPR 수행 시간
        int totalCount = cprTimestamps.Count;
        float rate = totalCount / duration;

        string result = rate >= 1.7f ? " [V] " : " [X] ";
        Debug.Log($"{result} CPR 속도: 총 {totalCount}회 / {duration:F1}초 → {rate:F2}회/초");

        EvaluateCPRDepth(); // 👈 깊이 평가 포함
    }

    // ✅ 깊이 분석: 평균 + 분포
    private void EvaluateCPRDepth()
    {
        if (cprDepths.Count == 0)
        {
            Debug.Log("[X] CPR 깊이 데이터가 없습니다.");
            return;
        }

        float sum = 0f;
        Dictionary<int, int> depthCounts = new Dictionary<int, int>();

        foreach (int d in cprDepths)
        {
            sum += d;
            if (!depthCounts.ContainsKey(d))
                depthCounts[d] = 1;
            else
                depthCounts[d]++;
        }

        float average = sum / cprDepths.Count;

        // 평균 깊이 판정 (기준: 4.5cm 이상)
        string resultMark = average >= 4.5f ? "[V]" : "[X]";
        Debug.Log($"{resultMark} CPR 평균 깊이: {average:F2}cm");

        // 세부 항목 출력
        string breakdown = "세부사항:";
        foreach (var pair in depthCounts)
        {
            breakdown += $" {pair.Key}cm: {pair.Value}회";
        }

        Debug.Log(breakdown);
    }
    */

    private string GetCPRFeedback()
    {
        if (cprTimestamps.Count == 0) return "";

        float duration = cprTimestamps[cprTimestamps.Count - 1];
        int totalCount = cprTimestamps.Count;
        float rate = totalCount / duration;
        string rateResult = rate >= 1.7f ? "[V]" : "[X]";

        float sum = 0f;
        SortedDictionary<int, int> depthCounts = new SortedDictionary<int, int>();
        foreach (int d in cprDepths)
        {
            sum += d;
            if (!depthCounts.ContainsKey(d))
                depthCounts[d] = 1;
            else
                depthCounts[d]++;
        }

        float avgDepth = sum / cprDepths.Count;
        string depthResult = avgDepth >= 4.5f ? "[V]" : "[X]";

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine($"CPR 속도: 총 {totalCount}회 / {duration:F1}초 → {rate:F2}회/초 {rateResult}");
        sb.AppendLine($"CPR 평균 깊이: {avgDepth:F2}cm {depthResult}");

        sb.Append("세부사항:");
        foreach (var pair in depthCounts)
        {
            sb.Append($" {pair.Key}cm: {pair.Value}회");
        }

        return sb.ToString();
    }

}
