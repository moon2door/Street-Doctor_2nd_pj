using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutorialStage { Intro, Move, Turn, Grab, Ray, Done }
    
public class TutorialManager : MonoBehaviour
{
    [Header("UI 오브젝트들")]
    public GameObject[] UI_start;
    public GameObject tutorialUI;
    public Text tutorialText;
    public Text countText;
    public GameObject successPanel;

    [Header("플레이어 제어")]
    public PlayerMove playerMove;
    public bool isMovementLocked = true; // 이동 잠금 상태

    [Header("트리거 오브젝트")]
    public GameObject moveTargetZone;
    public GameObject reverseStopZone;
    public GameObject cubeTargetZone;
    public GameObject buttonStopZone;

    [Header("칭찬 효과음")]
    public AudioClip praiseSFX;
    [Header("존 등장 효과음")]
    public AudioClip zoneAppearSFX;
    public AudioSource sfxAudioSource;
    //[Header("인트로 오디오")]
    //public AudioSource introAudio;
    //public float introDelay = 9f;    

    [Header("TTS 오디오 설정")]
    public AudioSource ttsAudioSource;
    public List<AudioClip> ttsClips;
    private Dictionary<string, AudioClip> ttsDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, string> ttsClipMap = new Dictionary<string, string>();

    [Header("튜토리얼 예시 오브젝트")]
    public GameObject exampleBlow;
    public GameObject exampleReverse;
    public GameObject exampleGrab;
    public GameObject exampleBtn;

    [Header("튜토리얼 상호작용 오브젝트")]
    public GameObject cubeObject;    

    int currentUIIndex = 0;
    public static TutorialManager Instance;
    public TutorialStage currentStage = TutorialStage.Intro;
    private bool hasEnteredCubeZone = false;
    private bool hasReachedButton = false;
    private bool canPressButton = false;
    public bool CanPressButton => canPressButton;
    private bool hasApproachedDoor = false;
    public static bool allowCubeStart = false;
    void Awake()
    {
        Instance = this;
        // TTS Dictionary 초기화
        foreach (var clip in ttsClips)
        {
            if (clip != null && !ttsDict.ContainsKey(clip.name))
                ttsDict.Add(clip.name, clip);
        }
        // 문구 → 파일명 매핑 초기화
        ttsClipMap.Add("안녕하세요\n슬기로운 응급생활에 오신 것을 환영합니다.", "tts_001");
        ttsClipMap.Add("이 프로그램은 CPR및 AED의 사용법을\n실제 상황처럼 학습할 수 있는\n몰입형 시뮬레이션입니다.", "tts_002");
        ttsClipMap.Add("슬기로운 응급생활은 컨트롤러 없이\n제스처만으로 진행됩니다.", "tts_003");
        ttsClipMap.Add("제스처 기능을 사용하기 위해\n컨트롤러를 내려 놓아 주세요.", "tts_004");
        ttsClipMap.Add("손을 올리면 여러분의 손동작에 따라\n제스처가 바뀌는 것을 보실 수 있습니다.", "tts_005");
        ttsClipMap.Add("지금부터 손동작에 따라\n이동하는 방법을 알려드리겠습니다.", "tts_006");
        ttsClipMap.Add("바로 보이는 손의 모양을 따라\n가슴 높이로 올리면 앞으로 이동합니다.", "tts_007");
        ttsClipMap.Add("잠시후 표시된 지점이 나타나면\n해당 위치로 이동해주세요.", "tts_008");
        ttsClipMap.Add("잘하셨어요~", "tts_009");
        ttsClipMap.Add("이번에는\n뒤로 이동하는 방법을 알려드리겠습니다.", "tts_010");
        ttsClipMap.Add("화면에 나온 손의 모양처럼\n양손을 뒤집으면 뒤로 갑니다.", "tts_011");
        ttsClipMap.Add("갑자기 뒤로가면\n어지러울 수 있으니 주의하세요.", "tts_012");
        ttsClipMap.Add("잘하셨어요!", "tts_013");
        ttsClipMap.Add("이제 물건을 잡아보겠습니다.", "tts_014");
        ttsClipMap.Add("표시된 지점이 나타나면\n해당 위치로 이동해주세요.", "tts_015");
        ttsClipMap.Add("눈 앞에 있는 루빅큐브를\n손으로 잡아보세요.", "tts_016");
        ttsClipMap.Add("앞의 손 모양을 참고하여\n잡아보세요.", "tts_026");
        ttsClipMap.Add("쥐고 있던 손은 펼치면\n큐브는 손에서 떨어집니다.", "tts_017");
        ttsClipMap.Add("이제 큐브를 들어\n바구니에 3번 넣어주세요.", "tts_018");
        ttsClipMap.Add("훌륭해요!", "tts_019");
        ttsClipMap.Add("마지막으로 버튼을 눌러보겠습니다.", "tts_020");
        ttsClipMap.Add("표시된 지점까지 이동해주세요.", "tts_021");        
        ttsClipMap.Add("버튼을 누르면\n앞에 있는 문이 열립니다.", "tts_023");
        ttsClipMap.Add("문이 열리면\n건물안으로 들어가\n응급처치 교육을 진행하겠습니다.", "tts_024");
        ttsClipMap.Add("앞에 있는 버튼을\n손으로 눌러주세요.", "tts_022");
        ttsClipMap.Add("저기 누군가 쓰러져있어요!\n가까이 다가가볼까요?", "tts_025");       
    }
    
void Start()
    {
        currentUIIndex = 0; // UI 인덱스 초기화
        StartCoroutine(PlayIntroSequence());
    }

    IEnumerator PlayIntroSequence()
    {
        isMovementLocked = true;

        yield return new WaitForSeconds(3f); 
        // 1단계: 인사말 자동 출력 (UI는 Debug.Log로 대체)
        yield return StartCoroutine(ShowUIWithTTS("안녕하세요\n슬기로운 응급생활에 오신 것을 환영합니다."));
        yield return StartCoroutine(ShowUIWithTTS("이 프로그램은 CPR및 AED의 사용법을\n실제 상황처럼 학습할 수 있는\n몰입형 시뮬레이션입니다."));
        yield return StartCoroutine(ShowUIWithTTS("슬기로운 응급생활은 컨트롤러 없이\n제스처만으로 진행됩니다."));
        yield return StartCoroutine(ShowUIWithTTS("제스처 기능을 사용하기 위해\n컨트롤러를 내려 놓아 주세요."));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(ShowUIWithTTS("손을 올리면 여러분의 손동작에 따라\n제스처가 바뀌는 것을 보실 수 있습니다."));        
        yield return StartCoroutine(ShowUIWithTTS("지금부터 손동작에 따라\n이동하는 방법을 알려드리겠습니다."));
        //yield return StartCoroutine(ShowUIWithTTS("각 단계의 안내 음성과\n화면 가이드를 따라 움직여 주십시오."));

        if (exampleBlow != null)
            exampleBlow.SetActive(true);

        yield return StartCoroutine(ShowUIWithTTS("바로 보이는 손의 모양을 따라\n가슴 높이로 올리면 앞으로 이동합니다."));
        yield return StartCoroutine(ShowUIWithTTS("잠시후 표시된 지점이 나타나면\n해당 위치로 이동해주세요."));

        if (moveTargetZone != null)
            moveTargetZone.SetActive(true); //  문구와 동시에 트리거 오브젝트 등장
        PlayZoneSFX(moveTargetZone.transform.position);

        isMovementLocked = false;
        HideUI(); //  UI 비활성화 함수가 있다면 호출
        currentStage = TutorialStage.Move;        
    }  
    
    public void OnPlayerTarget()
    {
        if (currentStage != TutorialStage.Move) return;

        isMovementLocked = true;
        if (moveTargetZone != null) moveTargetZone.SetActive(false);        

        StartCoroutine(HandlePostMove());
        currentStage = TutorialStage.Turn;
    }    
    IEnumerator HandlePostMove()
    {
        yield return StartCoroutine(ShowUIWithTTS("잘하셨어요~"));
        yield return StartCoroutine(ShowReverseTutorial());
    }
    IEnumerator ShowReverseTutorial()
    {        
        if (exampleBlow != null)
            exampleBlow.SetActive(false);

        if (exampleReverse != null)
            exampleReverse.SetActive(true);

        yield return StartCoroutine(ShowUIWithTTS("이번에는\n뒤로 이동하는 방법을 알려드리겠습니다."));
        yield return StartCoroutine(ShowUIWithTTS("화면에 나온 손의 모양처럼\n양손을 뒤집으면 뒤로 갑니다."));
        yield return StartCoroutine(ShowUIWithTTS("갑자기 뒤로가면\n어지러울 수 있으니 주의하세요."));

        if (reverseStopZone != null) reverseStopZone.SetActive(true);
        PlayZoneSFX(moveTargetZone.transform.position);
        isMovementLocked = false;
    }
    public void OnPlayerReverseTarget()
    {
        if (currentStage != TutorialStage.Turn) return;

        isMovementLocked = true;
        if (reverseStopZone != null) reverseStopZone.SetActive(false);
        if(exampleReverse != null) exampleReverse.SetActive(false);

        StartCoroutine(HandleReverseStop());
    }
    IEnumerator HandleReverseStop()
    {
        yield return StartCoroutine(ShowUIWithTTS("잘하셨어요!"));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(ShowUIWithTTS("이제 물건을 잡아보겠습니다."));
        yield return StartCoroutine(ShowUIWithTTS("표시된 지점이 나타나면\n해당 위치로 이동해주세요."));

        TutorialManager.allowCubeStart = true;
        if (cubeObject != null)
            cubeObject.SetActive(true); //  큐브 등장  
                
        CubeManager.Instance.StartCubeTask(); //  이제 안전하게 호출 
        HideUI();
        if (cubeTargetZone != null)
            cubeTargetZone.SetActive(true); //  큐브 트리거존 등장
        PlayZoneSFX(moveTargetZone.transform.position);

        currentStage = TutorialStage.Grab;
        isMovementLocked = false;    
    }
    public void OnPlayerReachedCubeZone()
    {
        if (hasEnteredCubeZone) return; //  이미 한 번 들어갔으면 무시
        hasEnteredCubeZone = true;
        
        if (currentStage != TutorialStage.Grab) return;

        isMovementLocked = true; 
        if (cubeTargetZone != null) cubeTargetZone.SetActive(false);

        StartCoroutine(HandleCubeGrabInstruction());
    }
    IEnumerator HandleCubeGrabInstruction()
    {        
        yield return StartCoroutine(ShowUIWithTTS("눈 앞에 있는 루빅큐브를\n손으로 잡아보세요."));
        if (exampleGrab != null)
        {
            exampleGrab.SetActive(true);
            Animator anim = exampleGrab.GetComponent<Animator>();
            if (anim != null)
            {
                anim.Play("Grab", 0, 0f);
            }
        }
        yield return StartCoroutine(ShowUIWithTTS("앞의 손 모양을 참고하여\n잡아보세요."));
        yield return StartCoroutine(ShowUIWithTTS("쥐고 있던 손은 펼치면\n큐브는 손에서 떨어집니다."));
        yield return StartCoroutine(ShowUIWithTTS("이제 큐브를 들어\n바구니에 3번 넣어주세요."));        
    }
    public void OnCubeMissionComplete()
    {
        if (successPanel != null)
            successPanel.SetActive(true);

        StartCoroutine(HandleCubeSuccessMessage());
    }
    IEnumerator HandleCubeSuccessMessage()
    {
        if (exampleGrab != null)
            exampleGrab.SetActive(false);
        yield return StartCoroutine(ShowUIWithTTS("훌륭해요!")); 
        yield return new WaitForSeconds(1f);
        StartCoroutine(NextTutorialStep());
    }
    IEnumerator NextTutorialStep()
    {
        yield return StartCoroutine(ShowUIWithTTS("마지막으로 버튼을 눌러보겠습니다."));
        yield return StartCoroutine(ShowUIWithTTS("표시된 지점까지 이동해주세요."));

        HideUI();

        if (buttonStopZone != null)
            buttonStopZone.SetActive(true);  //  트리거존 등장
        PlayZoneSFX(moveTargetZone.transform.position);

        currentStage = TutorialStage.Ray;  // 단계 전환
        isMovementLocked = false;        
    }
    public void OnReachedButtonZone()
    {
        if (hasReachedButton) return;  //  중복 진입 방지
        hasReachedButton = true;

        isMovementLocked = true;
        if (buttonStopZone != null)
        {
            buttonStopZone.SetActive(false);           
        }
        StartCoroutine(ButtonPreInstruction());
    }
    IEnumerator ButtonPreInstruction()
    {
        if (exampleBtn != null)
            exampleBtn.SetActive(true);
        Animator anime = exampleBtn.GetComponent<Animator>();
        if (anime != null)
        {
            anime.Play("touchhand", 0, 0f);
        }
        yield return StartCoroutine(ShowUIWithTTS("문이 열리면\n건물안으로 들어가\n응급처치 교육을 진행하겠습니다."));
        yield return StartCoroutine(ShowUIWithTTS("버튼을 누르면\n앞에 있는 문이 열립니다."));
        yield return StartCoroutine(ShowUIWithTTS("앞에 있는 버튼을\n손으로 눌러주세요."));
        if (exampleBtn != null)
            exampleBtn.SetActive(false);
        canPressButton = true;
        isMovementLocked = false;
    }
    public void OnApproachDoor()
    {
        if (hasApproachedDoor) return;
        hasApproachedDoor = true;

        isMovementLocked = true;
        GameObject doorZone = GameObject.Find("Door Zone");
        if (doorZone != null)
        {
            doorZone.SetActive(false);            
        }
        StartCoroutine(DoorApproachSequence());
    }
    IEnumerator DoorApproachSequence()
    {
        PlayZoneSFX(moveTargetZone.transform.position);
        yield return StartCoroutine(ShowUIWithTTS("저기 누군가 쓰러져있어요!\n가까이 다가가볼까요?"));

        yield return new WaitForSeconds(1.5f);  

        isMovementLocked = false;  
        currentStage = TutorialStage.Done;      // 튜토리얼 종료
        DisableTutorialMode();                  // GrabbableObject isTutorialObject = false
        HideUI();
    }    
    void DisableTutorialMode()
    {       
        GrabbableObject[] objs = FindObjectsOfType<GrabbableObject>();
        foreach (var obj in objs)
        {
            obj.isTutorialObject = false;
        }
    }
    void PlayZoneSFX(Vector3 position)
    {
        if (zoneAppearSFX != null)
            AudioSource.PlayClipAtPoint(zoneAppearSFX, position);
    }
    IEnumerator ShowUIWithTTS(string message)
    {        
        ShowUI(message);

        if ((message.Contains("잘하셨어요~") || message.Contains("잘하셨어요!") || message.Contains("훌륭해요!")) &&
        praiseSFX != null && sfxAudioSource != null)
        {
            sfxAudioSource.PlayOneShot(praiseSFX);
        }

        if (ttsClipMap.TryGetValue(message, out string clipName) && ttsDict.TryGetValue(clipName, out AudioClip clip))
        {
            ttsAudioSource.Stop();
            ttsAudioSource.PlayOneShot(clip);            
            yield return new WaitForSeconds(clip.length + 1f); // 완전한 텀으로 합치기            
        }
        else
        {
            yield return new WaitForSeconds(4f);
        }
    }    
    IEnumerator _Chat(string _chat, string _secondChat) // <- 추가
    {
        ShowUI(_chat);
        yield return new WaitForSeconds(3f);
        ShowUI(_secondChat);
    }    
    void ShowUI(string message)
    {
        if (tutorialUI != null) tutorialUI.SetActive(true);
        foreach (var ui in UI_start) ui?.SetActive(false);
        if (currentUIIndex < UI_start.Length && UI_start[currentUIIndex] != null)
        {
            UI_start[currentUIIndex].SetActive(true);
            var txt = UI_start[currentUIIndex].GetComponentInChildren<Text>();
            if (txt != null) txt.text = message;
        }
        currentUIIndex++;
    }
    void HideUI()
    {
        foreach (var ui in UI_start)
        {
            if (ui != null)
                ui.SetActive(false);
        }
        if (tutorialUI != null)
            tutorialUI.SetActive(false);
    }
    public void UpdateCubeUI(int current, int goal)
    {
        if (countText != null)
            countText.text = $"{current} / {goal}";
    }       
    public bool IsMovementAllowed()
    {
        return !isMovementLocked;
    }
}
