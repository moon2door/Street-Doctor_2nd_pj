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
    private float cprStartTime = -1f;
    private List<float> cprTimestamps = new List<float>();
    private List<int> cprDepths = new List<int>();

    [Header("UI 출력")]
    public Text feedbackText;
    public GameObject uiOBJ;

    public AudioSource myAudio;
    public AudioClip uiClip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (playerTransform == null)
        {
            GameObject target = GameObject.Find("cpr_obj_onoff");

            if (target != null && target.activeInHierarchy)
            {
                playerTransform = target.transform;
            }
        }
    }

    public void RecordCPR(int depthCm)
    {
        if (cprStartTime < 0)
            cprStartTime = Time.time;

        float relativeTime = Time.time - cprStartTime;
        cprTimestamps.Add(relativeTime);
        cprDepths.Add(depthCm);
    }

    public void AddAEDButton(string btnName)
    {
        aedSequence.Add(btnName);
    }

    public void SetPadsPlaced(bool placed)
    {
        padsPlacedCorrectly = placed;
    }

    public void PrintFeedback()
    {
        carResponder.ActivateResponse(playerTransform);

        uiOBJ.SetActive(true);
        myAudio.PlayOneShot(uiClip);

        string GreenV = "<color=green>[V]</color>";
        string RedX = "<color=red>[X]</color>";

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine($"어깨를 두드려서 의식을 확인했나요? {(didCheckConscious ? GreenV : RedX)}");
        sb.AppendLine($"주변 사람을 가리켜 지시를 했나요? {(didCallHelp ? GreenV : RedX)}");

        if (cprTimestamps.Count == 0)
        {
            sb.AppendLine($"CPR을 수행했나요? {RedX}");
        }
        else
        {
            sb.AppendLine($"CPR을 수행했나요? {GreenV}");
            sb.AppendLine(GetCPRFeedback());
        }

        bool aedCorrect = IsSequenceCorrect(new List<string> { "BtnOpen", "BtnR", "BtnShock" });
        sb.AppendLine($"AED 버튼을 순서대로 눌렀나요? {(aedCorrect ? GreenV : RedX)}");
        sb.AppendLine($"패드를 올바른 위치에 부착했나요? {(padsPlacedCorrectly ? GreenV : RedX)}");

        if (feedbackText != null)
            feedbackText.text = sb.ToString();
    }

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

    private string GetCPRFeedback()
    {
        if (cprTimestamps.Count == 0) return "";

        float duration = cprTimestamps[cprTimestamps.Count - 1];
        int totalCount = cprTimestamps.Count;
        float rate = totalCount / duration;
        string rateResult = rate >= 1.7f ? "<color=green>[V]</color>" : "<color=red>[X]</color>";

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
        string depthResult = avgDepth >= 4.5f ? "<color=green>[V]</color>" : "<color=red>[X]</color>";

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine($"CPR 속도: 총 {totalCount}회 / {duration:F1}초 → {rate:F2}회/초 {rateResult}");
        sb.AppendLine($"CPR 평균 깊이: {avgDepth:F2}cm {depthResult}");

        sb.Append("세부사항:");
        foreach (var pair in depthCounts)
        {
            sb.Append($" {pair.Key}cm→{pair.Value}회");
        }

        return sb.ToString();
    }
}
