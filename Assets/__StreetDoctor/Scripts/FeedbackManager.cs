using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackManager : MonoBehaviour
{
    public GameObject correctUI;
    public GameObject incorrectUI;
    public AudioSource audioSource;
    public AudioClip correctClip;
    public AudioClip incorrectClip;
    public float vibrationStrength = 0.5f;
    public float vibrationDuration = 0.2f;

    public void ShowFeedback(bool success)
    {
        if (success)
        {
            if (correctUI) correctUI.SetActive(true);
            if (correctClip) audioSource.PlayOneShot(correctClip);
            Invoke(nameof(HideCorrectUI), 2f);
        }
        else
        {
            if (incorrectUI) incorrectUI.SetActive(true);
            if (incorrectClip) audioSource.PlayOneShot(incorrectClip);
            Invoke(nameof(HideIncorrectUI), 2f);
        }

        StartCoroutine(Vibrate());
    }

    private System.Collections.IEnumerator Vibrate()
    {
        OVRInput.SetControllerVibration(1f, vibrationStrength, OVRInput.Controller.RTouch);
        yield return new WaitForSeconds(vibrationDuration);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
    }

    void HideCorrectUI() { if (correctUI) correctUI.SetActive(false); }
    void HideIncorrectUI() { if (incorrectUI) incorrectUI.SetActive(false); }
}
