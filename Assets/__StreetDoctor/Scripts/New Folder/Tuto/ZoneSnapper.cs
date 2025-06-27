using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneSnapper : MonoBehaviour
{
    [Header("올바른 오브젝트 태그")]
    public string grabbableTag = "Grabbable";

    [Header("피드백 색상")]
    public Color highlightColor = Color.red;
    private Color originalColor;

    private Renderer rend;
    private bool hasSnapped = false;
    private void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null && rend.material.HasProperty("_BaseColor"))
        {
            originalColor = rend.material.GetColor("_BaseColor");
        }
    }
    public void SnapObject(GameObject obj)
    {
        // 스냅
        obj.transform.SetParent(null); // 혹시 모르니 부모 끊기
        obj.transform.position = transform.position;
        
        // 색상 강조 유지
        if (rend != null && rend.material.HasProperty("_BaseColor"))
        {
            var c = highlightColor;
            c.a = 1f;
            rend.material.SetColor("_BaseColor", c);
        }
    }

    public void ResetColor()
    {
        if (rend != null && rend.material.HasProperty("_BaseColor"))
        {
            rend.material.SetColor("_BaseColor", originalColor);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(grabbableTag)) return;
        var c = highlightColor; c.a = 1f;
        rend.material.SetColor("_BaseColor", c);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(grabbableTag)) return;
        ResetColor();
    }
}
