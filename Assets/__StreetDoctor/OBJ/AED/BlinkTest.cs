using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkTest : MonoBehaviour
{
    public Material targetMat; // ÀÎ½ºÆåÅÍ¿¡¼­ AED_open µî ÇÒ´ç
    public Color baseColor = Color.white;
    public Color blinkColor = new Color(0f, 1f, 0.6f); // Çü±¤ ¹ÎÆ®
    public float blinkInterval = 0.5f;

    private bool isOn = false;

    void Start()
    {
        InvokeRepeating(nameof(ToggleColor), 0f, blinkInterval);
    }

    void ToggleColor()
    {
        if (targetMat == null) return;

        targetMat.color = isOn ? baseColor : blinkColor;
        isOn = !isOn;
    }
}
