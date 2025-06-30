using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutorialStage
{
    Intro,
    Move,
    Turn,
    Grab,
    Ray,
    Done
}
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

    [Header("트리거 오브젝트")]
    public GameObject moveTargetZone;

    [Header("인트로 오디오")]
    public AudioSource introAudio;
    public float introDelay = 9f;

    [Header("튜토리얼 진행 상태")]
    public TutorialStage currentStage = TutorialStage.Intro;
   
    int currentUIIndex = 0;
    public static TutorialManager Instance;
    
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {        
        StartCoroutine(PlayIntroSequence());
    }

    IEnumerator PlayIntroSequence()
    {
        // 1단계: 인사말 자동 출력 (UI는 Debug.Log로 대체)
        ShowUI("안녕하세요!");
        yield return new WaitForSeconds(3f);

        ShowUI("이 튜토리얼은 VR 조작법을 배우기 위한 단계입니다.");
        yield return new WaitForSeconds(3f);

        ShowUI("왼손 스틱을 이용해 화살표로 표시되어 있는 지점까지 움직여 보세요");
        yield return new WaitForSeconds(3f);

        if (moveTargetZone != null)
            moveTargetZone.SetActive(true); //  문구와 동시에 트리거 오브젝트 등장

        HideUI(); //  UI 비활성화 함수가 있다면 호출
        currentStage = TutorialStage.Move;
        //EnableMovement();
    }
    
    IEnumerator _Chat(string _chat, string _secondChat) // <- 추가
    {
        ShowUI(_chat);
        yield return new WaitForSeconds(3f);
        ShowUI(_secondChat);
    }
    IEnumerator _ChatWithCubeTask(string message1, string message2)
    {
        ShowUI(message1);
        yield return new WaitForSeconds(3f);

        // 바구니 & 큐브 활성화 및 시작
        CubeManager.Instance.StartCubeTask();
        yield return new WaitForSeconds(0.5f); // 약간의 여유

        ShowUI(message2);
        yield return new WaitForSeconds(3f);
    }
    public void OnPlayerTarget()
    {
        if (currentStage != TutorialStage.Move) return;       
        // 오브젝트 제거        
        Destroy(moveTargetZone, 0.2f);
        // UI 출력
        StartCoroutine(_Chat("잘하셨어요!", "이번엔 오른손 스틱으로 시야를 돌려보세요."));
        // 상태 전환
        currentStage = TutorialStage.Turn;
    }   
    public void OnPlayerTurnedEnough()
    {
        if (currentStage != TutorialStage.Turn) return;

        StartCoroutine(_Chat("잘하셨어요!", "큐브를 바구니에 3개 넣어보세요."));
        currentStage = TutorialStage.Grab;        
    }
    public void OnPlayerGrabdone()
    {
        if (currentStage != TutorialStage.Grab) return;

        StartCoroutine(_Chat("잘하셨어요!", "물건을 조준해서 검지를 눌러보세요."));
        currentStage = TutorialStage.Ray;       
    }
    public void OnGrabSuccess()
    {
        if (currentStage != TutorialStage.Ray) return;
        StartCoroutine(_Chat("튜토리얼 완료!", "다음 단계로 이동합니다!"));
        currentStage = TutorialStage.Done;
    }

    public void UpdateCubeUI(int current, int goal)
    {
        if (countText != null)
            countText.text = $"{current} / {goal}";
    }

    public void OnCubeMissionComplete()
    {
        if (successPanel != null)
            successPanel.SetActive(true);

        StartCoroutine(NextTutorialStep());
        // 추가 연출이나 다음 단계 전환 가능
    }
    IEnumerator NextTutorialStep()
    {
        yield return new WaitForSeconds(2f);
        ShowUI("튜토리얼 마지막 단계로 이동합니다.");
        yield return new WaitForSeconds(2f);

        currentStage = TutorialStage.Ray;
    }
    //UI 안내 텍스트(향후 TextMeshPro로 교체 가능)
    void ShowUI(string message)
    {
        // 모든 UI 오브젝트 비활성화 (필요 시 유지)
        foreach (var ui in UI_start)
        {
            if (ui != null)
                ui.SetActive(false);
        }
        // 현재 인덱스의 UI 오브젝트 활성화
        if (currentUIIndex < UI_start.Length && UI_start[currentUIIndex] != null)
        {
            UI_start[currentUIIndex].SetActive(true);
            // 해당 UI 안의 텍스트에 메시지 출력
            Text txt = UI_start[currentUIIndex].GetComponentInChildren<Text>();
            if (txt != null)
                txt.text = message;
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
}
