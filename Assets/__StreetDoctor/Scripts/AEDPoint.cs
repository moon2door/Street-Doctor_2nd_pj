using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AEDPoint : MonoBehaviour
{
    public int slotID = 1;
    public Renderer meshRenderer;
    public Color correctColor = Color.green;
    public Color incorrectColor = Color.red;
    public Color normalColor = Color.white;

    public void ShowFeedback(bool correct)
    {
        if (meshRenderer != null)
            meshRenderer.material.color = correct ? correctColor : incorrectColor;
    }

    public void ResetColor()
    {
        if (meshRenderer != null)
            meshRenderer.material.color = normalColor;
    }
}
