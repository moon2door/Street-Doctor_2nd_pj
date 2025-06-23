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

    [Header("플레이어 입력 제어")]
    //public Grab leftGrab;
    //public Grab rightGrab;

    [Header("이동 트리거 오브젝트")]
    public GameObject moveTargetZone;

    [Header("인트로 오디오")]
    public AudioSource introAudio;
    public float introDelay = 9f;

    [Header("튜토리얼 진행 상태")]
    public TutorialStage currentStage = TutorialStage.Intro;

    [Header("튜토리얼 UI")]
    public GameObject tutorialUI;
    public Text tutorialText;

    int currentUIIndex = 0;

    void Start()
    {
        //DisableAllControls();
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
        yield return new WaitForSeconds(3f); //김승수 주석처리함.


        //if (moveTargetZone != null)
        //    moveTargetZone.SetActive(true); //  문구와 동시에 트리거 오브젝트 등장
        
        HideUI(); //  UI 비활성화 함수가 있다면 호출
        if (moveTargetZone != null)
            moveTargetZone.SetActive(true);

        currentStage = TutorialStage.Move;
        //EnableMovement();

        void HideUI()
        {
            if (tutorialUI != null)
                tutorialUI.SetActive(false);
        }


        // 이동만 허용
        currentStage = TutorialStage.Move;
        //EnableMovement();
    }
    IEnumerator _Chat(string _chat, string _secondChat) // <- 추가
    {
        ShowUI(_chat);
        yield return new WaitForSeconds(3f);
        ShowUI(_secondChat);
    }

    public void OnPlayerTarget()
    {
        if (currentStage != TutorialStage.Move) return;

        //DisableMovement();
        StartCoroutine(_Chat("잘하셨어요!", "이번엔 오른손 스틱으로 시야를 돌려보세요."));
        currentStage = TutorialStage.Turn;
        //EnableRotation();
    }
    
    public void OnPlayerTurnedEnough()
    {
        if (currentStage != TutorialStage.Turn) return;

        StartCoroutine(_Chat("잘하셨어요!", "물건을 표시된 곳에 놓아보세요."));
        currentStage = TutorialStage.Grab;
        //EnableGrab();
    }

    public void OnPlayerGrabdone()
    {
        if (currentStage != TutorialStage.Grab) return;

        StartCoroutine(_Chat("잘하셨어요!", "물건을 조준해서 검지를 눌러보세요."));
        currentStage = TutorialStage.Ray;
        //EnableRay();
    }

    public void OnGrabSuccess()
    {
        if (currentStage != TutorialStage.Ray) return;
        StartCoroutine(_Chat("튜토리얼 완료!", "다음 단계로 이동합니다!"));
        currentStage = TutorialStage.Done;
    }

    /*
    // ------------------------------
    //  입력 제어 함수들
    void DisableAllControls()
    {
        if (playerMove != null)
        {
            playerMove.enabled = false;
            playerMove.SetRotationEnabled(false);
        }

        if (leftGrab != null) leftGrab.enabled = false;
        if (rightGrab != null) rightGrab.enabled = false;
    }

    void DisableMovement()
    {
        if (playerMove != null) playerMove.enabled = false;
    }

    void EnableMovement()
    {
        if (playerMove != null) playerMove.enabled = true;    
    }

    void EnableRotation()
    {
        if (playerMove != null) playerMove.SetRotationEnabled(true);
    }

    void EnableGrab()
    {
        if (leftGrab != null) leftGrab.enabled = true;
        if (rightGrab != null) rightGrab.enabled = true;
    }

    void EnableRay()
    {
        // 레이 실행되도록 함수 추가하기
    }

    void HandleTurned() // <- 추가
    {
        if (currentStage == TutorialStage.Turn)
        {
            OnPlayerTurnedEnough();
            playerMove.OnTurned -= HandleTurned; // 한 번만 호출되게 제거
        }
    }
    */

    // ------------------------------
    //  UI 안내 텍스트 (향후 TextMeshPro로 교체 가능)
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
}
