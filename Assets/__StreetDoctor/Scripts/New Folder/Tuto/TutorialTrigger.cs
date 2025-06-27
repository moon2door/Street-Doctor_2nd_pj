using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [Header("도착 시 사라질 오브젝트")]
    public GameObject targetObject;

    //[Header("색상만 변경할 오브젝트")]
    //public Renderer objectRenderer;
    //public Color successColor = Color.green;

    private bool triggered = false;

    private void Start()
    {
        targetObject = GameObject.Find("TriggerZone");
        //objectRenderer = GameObject.Find("TriggerZone");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        // 조건 충족 → 튜토리얼 매니저에 알림만
        TutorialManager.Instance.OnPlayerTarget();
    }
 } 
