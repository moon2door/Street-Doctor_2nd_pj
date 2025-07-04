using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CPRTraningStart : MonoBehaviour
{
    [Header("단일 텍스트 UI 오브젝트")]
    public Text uiText;

    [Header("포탈")]
    public GameObject portal;

    [Header("각 단계별 텍스트 내용 / 오디오 클립과 순서 맞추세요.")]
    public string[] stepTexts;

    [Header("오디오 클립 배열 / 텍스트와 순서를 반드시 맞추세요.")]
    public AudioClip[] audioClips;

    private bool isTriggered = false;
    private int stepIndex = 0;

    [Header("UI 조절 bool 값 / 클립과 순서를 반드시 맞춰주세요.")]
    public bool[] stepConditions;

    [Header("오디오 소스")]
    public AudioSource audioSource;

    [Header("몇 번째 단계까지 있는지?")]
    public int stepCount;

    [Header("소환할 NPC 오브젝트들")]
    public GameObject[] spawnNPC;

    [Header("어깨 두드리는 손 오브젝트")]
    public GameObject shoulderHand;
    public GameObject shoulderOBJ;

    [Header("휴대폰")]
    public GameObject phoneOBJ;

    [Header("가슴 압박하는 손 오브젝트")]
    public GameObject cprHand;
    public GameObject cprOBJ;

    [Header("모든 단계 완료 후 활성화할 오브젝트")]
    public GameObject finalObject;

    [Header("cpr타이머와 횟수")]
    public GameObject cprUI;
    private CPRTimer cprTimer;

    [Header("옷")]
    public GameObject clothOBJ;
    public GameObject clothCol;

    private void Start()
    {
        cprTimer = GetComponent<CPRTimer>();
    }

    void Update()
    {
        if (!isTriggered) return;

        isTriggered = false;
        StartCoroutine(PlayFullSequence());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTriggered = true;
            portal.SetActive(false);
        }
    }

    public IEnumerator PlayFullSequence()
    {
        for (int i = 0; i < stepCount; i++)
        {
            yield return StartCoroutine(PlayStep(() => stepConditions[i]));
        }

        if (finalObject != null)
        {
            finalObject.SetActive(true);
        }
    }

    public IEnumerator PlayStep(System.Func<bool> triggerCheck)
    {
        int waitIndex = stepIndex;  // 디버깅용 인덱스 복사

        Debug.Log($"[Step {waitIndex}] 대기 시작");

        yield return new WaitUntil(() => triggerCheck());

        Debug.Log($"[Step {waitIndex}] 조건 만족. 진행 시작");

        if (stepIndex >= Mathf.Min(stepTexts.Length, audioClips.Length))
            yield break;

        int currentIndex = stepIndex;
        stepIndex++;

        // 문구가 시작하기 전 이벤트 실행
        switch (currentIndex)
        {
            case 2:
                OBJ_ActiveSelf(shoulderHand);
                OBJ_ActiveSelf(shoulderOBJ);
                break;
            case 4:
                OBJ_ActiveSelf(shoulderHand);
                OBJ_ActiveSelf(shoulderOBJ);
                break;
            case 5:
                SpawnNPC();
                break;
            case 10:
                OBJ_ActiveSelf(phoneOBJ);
                break;
            case 13:
                OBJ_ActiveSelf(phoneOBJ);
                break;
            case 21:
                OBJ_ActiveSelf(cprHand);
                OBJ_ActiveSelf(cprOBJ);
                break;
            case 23:
                OBJ_ActiveSelf(cprHand);
                OBJ_ActiveSelf(cprOBJ);
                break;
            case 26:
                OBJ_ActiveSelf(cprUI);
                break;
        }

        // 텍스트 표시
        if (uiText != null && currentIndex < stepTexts.Length)
            uiText.text = stepTexts[currentIndex];

        // 오디오 재생
        if (audioSource != null && audioClips[currentIndex] != null)
        {
            audioSource.clip = audioClips[currentIndex];
            audioSource.Play();
            yield return new WaitForSeconds(audioClips[currentIndex].length);
        }

        yield return new WaitForSeconds(1f);

        // 문구가 끝나고 이벤트 실행
        switch (currentIndex)
        {
            case 25:
                OBJ_ActiveSelf(cprUI);
                cprTimer.TimerStart();
                break;
            case 29:
                OBJ_ActiveSelf(clothCol);
                break;
        }
    }

    // 외부에서 특정 단계 실행 가능
    public void TriggerStep(int index)
    {
        if (index >= 0 && index < stepConditions.Length)
        {
            stepConditions[index] = true;
        }
    }

    void OBJ_ActiveSelf(GameObject obj)
    {
        if (obj.activeSelf)
        {
            obj.SetActive(false);
        }
        else
        {
            obj.SetActive(true);
        }
    }

    void SpawnNPC()
    {
        foreach (GameObject npc in spawnNPC)
        {
            npc.SetActive(true);
        }
    }
}
