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
    private CapsuleCollider mycoll;

    [Header("UI 조절 bool 값 / 클립과 순서를 반드시 맞춰주세요.")]
    public bool[] stepConditions;

    [Header("오디오 소스")]
    public AudioSource audioSource;
    public AudioClip clearSound;
    public AudioClip bellSound;
    public AudioClip emergencySound;
    public AudioClip shockSound;    

    [Header("몇 번째 단계까지 있는지?")]
    public int stepCount;

    [Header("소환할 NPC 오브젝트들")]
    public GameObject[] spawnNPC;

    [Header("cpr애니메이션 NPC")]
    public GameObject cprnpcOBJ;

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
    public GameObject timerUI;
    public GameObject gaugeUI;
    private CPRTimer cprTimer;

    [Header("옷")]
    public GameObject clothOBJ;
    public GameObject clothCol;

    [Header("AED")]
    public GameObject aedOBJ;
    public GameObject pad1_OBJ;
    public GameObject pad2_OBJ;

    // AED버튼에 에미션 효과를 주기 위한 변수
    [Header("에미션 머터리얼)")]
    public Material btnOpen;
    public Material btnR;
    public Material btnShock;

    private bool isOpenOn = false;
    private bool isROn = false;
    private bool isShockOn = false;

    private Color defaultColor = Color.white;

    public TutorialManager tutorialManager;

    private void Start()
    {
        mycoll = GetComponent<CapsuleCollider>();
        cprTimer = GetComponent<CPRTimer>();        
    }

    void Update()
    {
        if (!isTriggered) return;

        isTriggered = false;
        tutorialManager.isMovementLocked = true;
        mycoll.enabled = false;
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
    }

    public IEnumerator PlayStep(System.Func<bool> triggerCheck)
    {
        int waitIndex = stepIndex;
        Debug.Log($"[Step {waitIndex}] 대기 시작");

        yield return new WaitUntil(() => triggerCheck());

        Debug.Log($"[Step {waitIndex}] 조건 만족. 진행 시작");

        if (stepIndex >= Mathf.Min(stepTexts.Length, audioClips.Length))
            yield break;

        int currentIndex = stepIndex;
        stepIndex++;

        // 🎵 특정 단계에서 클리어 사운드 재생
        int[] playClearSteps = { 3, 7, 12, 22, 30, 32, 33, 34, 41 }; //숫자 변경
        if (System.Array.Exists(playClearSteps, step => step == currentIndex))
        {
            if (audioSource != null && clearSound != null)
                audioSource.PlayOneShot(clearSound);
        }

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
             //119연결음
            case 12:
                if (audioSource != null && bellSound != null)
                {
                    audioSource.PlayOneShot(bellSound);
                    yield return new WaitForSeconds(bellSound.length);
                }
                yield return new WaitForSeconds(bellSound.length + 0.2f);
                if (audioSource != null && emergencySound != null)
                {
                    audioSource.PlayOneShot(emergencySound);
                    yield return new WaitForSeconds(emergencySound.length);
                }
                yield return new WaitForSeconds(2f);
                break;
            case 13:
                OBJ_ActiveSelf(phoneOBJ);
                break;
            case 21:
            case 23:
                OBJ_ActiveSelf(cprHand);
                OBJ_ActiveSelf(cprOBJ);
                OBJ_ActiveSelf(cprnpcOBJ);
                break;
            case 26:
                OBJ_ActiveSelf(timerUI);
                OBJ_ActiveSelf(cprnpcOBJ);
                break;
            case 31:
                OBJ_ActiveSelf(aedOBJ);                
                break;
            case 33:                
                StopEmission();//Emission
                break;
            case 34:
                StopEmission();//Emission
                break;
            case 35:
                OBJ_ActiveSelf(pad1_OBJ);
                OBJ_ActiveSelf(pad2_OBJ);
                break;
            case 40:
                StopEmission();//Emission
                break;
            case 43:
                OBJ_ActiveSelf(finalObject);
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
            case 11:                
                break;
            case 25:
                OBJ_ActiveSelf(timerUI);
                cprTimer.TimerStart();
                break;
            case 29:
                OBJ_ActiveSelf(clothCol);
                break;
            case 31:
                //Emission
                if (btnOpen != null)
                {
                    btnOpen.color = defaultColor;
                    InvokeRepeating(nameof(ToggleOpen), 0f, 0.5f);
                }
                break;
            case 32:
                //Emission
                if (btnR != null)
                {
                    btnR.color = defaultColor;
                    InvokeRepeating(nameof(TogglePower), 0f, 0.5f);
                }
                break;
            case 38:
                //Emission
                if (btnShock != null)
                {
                    btnShock.color = defaultColor;
                    InvokeRepeating(nameof(ToggleShock), 0f, 0.5f);
                }
                break;
            case 39:
                //전기충격효과음
                if (audioSource != null && shockSound != null)
                {
                    audioSource.PlayOneShot(shockSound);
                    yield return new WaitForSeconds(shockSound.length);
                }
                break;
            case 40:
                OBJ_ActiveSelf(aedOBJ);
                break;
            case 44:
                tutorialManager.isMovementLocked = false;
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

    //Emission on/off 계속
    void ToggleOpen()
    {
        if (btnOpen == null) return;
        btnOpen.color = isOpenOn ? defaultColor : new Color(1f, 0.2f, 0.2f); // 빨강
        isOpenOn = !isOpenOn;
    }
    void TogglePower()
    {
        if (btnR == null) return;
        btnR.color = isROn ? defaultColor : new Color(0f, 1f, 0.5f); // 청록
        isROn = !isROn;
    }
    void ToggleShock()
    {
        if (btnShock == null) return;
        btnShock.color = isShockOn ? defaultColor : new Color(1f, 0.2f, 0.2f); // 진빨강
        isShockOn = !isShockOn;
    }
    void StopEmission()
    {
        CancelInvoke(nameof(ToggleOpen));
        CancelInvoke(nameof(TogglePower));
        CancelInvoke(nameof(ToggleShock));

        if (btnOpen != null) btnOpen.color = defaultColor;
        if (btnR != null) btnR.color = defaultColor;
        if (btnShock != null) btnShock.color = defaultColor;

        isOpenOn = false;
        isROn = false;
        isShockOn = false;
    }   
}
