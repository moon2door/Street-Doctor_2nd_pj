using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class HandPalmMove : MonoBehaviour
{
    [Header("참조")]
    public Transform leftHand;
    public Transform cameraRig;
    public OVRHand ovrHand;          // ← 손가락 상태 확인용

    public Transform rightHand;
    public OVRHand ovrHandRight;

    [Header("설정")]
    public float moveSpeed = 1.5f;
    public float palmMoveThreshold = 0.5f;

    [Header("소리")]
    public AudioSource footstepAudio;
    public AudioClip footstepClip;


    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        if (cameraRig == null)
            cameraRig = Camera.main.transform;

        leftHand = GameObject.Find("LeftHandAnchor").transform;
        cameraRig = GameObject.Find("CenterEyeAnchor").transform;
        ovrHand = GameObject.Find("OVRLeftHandDataSource").GetComponent<OVRHand>();

        rightHand = GameObject.Find("RightHandAnchor").transform;
        ovrHandRight = GameObject.Find("OVRRightHandDataSource").GetComponent<OVRHand>();

        footstepAudio = GetComponent<AudioSource>();

    }

    void Update()
    {
        // 튜토리얼 진행 중 이동 잠금 확인
        if (TutorialManager.Instance != null && !TutorialManager.Instance.IsMovementAllowed())
            return;

        if (!IsFist())
        {
            StopFootstepSound();
            return;
        }

        float upDotL = Vector3.Dot(leftHand.up, Vector3.up);
        float upDotR = Vector3.Dot(rightHand.up, Vector3.up);

        Vector3 moveDir = Vector3.zero;

        if (upDotL < -0.5f && upDotR < -0.5f)
        {
            // 양 손목이 하늘을 향함 → 뒤로
            moveDir = -leftHand.forward;
        }
        else if (upDotL > 0.5f && upDotR > 0.5f)
        {
            // 양 손목이 바닥을 향함 → 앞으로
            moveDir = leftHand.forward;
        }

        moveDir.y = 0;

        if (moveDir != Vector3.zero)
        {
            characterController.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
            PlayFootstepSound(); // 이동 시 발소리 재생
        }
        else
        {
            StopFootstepSound(); // 멈출 경우 소리 정지
        }
    }

    void PlayFootstepSound()
    {
        if (footstepAudio != null && footstepClip != null && !footstepAudio.isPlaying)
        {
            footstepAudio.clip = footstepClip;
            footstepAudio.loop = true;
            footstepAudio.Play();
        }
    }

    void StopFootstepSound()
    {
        if (footstepAudio != null && footstepAudio.isPlaying)
        {
            footstepAudio.Stop();
        }
    }

    // 튜토리얼
    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Trigger Zone") // 전방 도착 처리
        {
            TutorialManager.Instance.OnPlayerTarget();
        }
        else if (other.name == "ReverseStopZone") // 뒤로가기 도착 처리
        {
            TutorialManager.Instance.OnPlayerReverseTarget();
        }
        else if (other.name == "Cube Zone") // 큐브앞 도착 처리
        {
            TutorialManager.Instance.OnPlayerReachedCubeZone();
        }
        else if (other.name == "Button Zone") // 버튼 앞 도착 처리
        {
            TutorialManager.Instance.OnReachedButtonZone(); 
        }
        else if (other.name == "Door Zone") // 문앞 존 감지
        {
            TutorialManager.Instance.OnApproachDoor();
        }
    }
    bool IsFist()
    {
        float threshold = 0.3f;

        // 왼손
        float indexL = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
        float middleL = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Middle);

        // 오른손
        float indexR = ovrHandRight.GetFingerPinchStrength(OVRHand.HandFinger.Index);
        float middleR = ovrHandRight.GetFingerPinchStrength(OVRHand.HandFinger.Middle);

        Debug.Log($"검지:{indexL:F2}, 중지:{middleL:F2} || 검지:{indexR:F2}, 중지:{middleR:F2}");

        return indexL > threshold && middleL > threshold && indexR > threshold && middleR > threshold;
    }    
}
