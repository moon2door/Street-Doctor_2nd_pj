using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CPRTimer : MonoBehaviour
{
    public Text timerText;
    public Text cprCount;
    public CPRMain cprMain;

    public AudioSource audioSource;
    public AudioClip failAudioClip;
    public AudioClip completeClip;
    public AudioClip clearClip;

    private CPRTraningStart myTR;

    public float timerSec;
    private float timerSecF1;
    private float failedCount = 0f;

    private bool isTimerRunning = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        myTR = GetComponent<CPRTraningStart>();

        cprMain = GameObject.Find("XRHand_PalmR").GetComponent<CPRMain>();
        timerSecF1 = timerSec;
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            timerSec -= Time.deltaTime;

            if (timerSec <= 0)
            {
                timerSec = 0;
                isTimerRunning = false;

                // 타이머 종료 후 CPR 횟수 판정
                if (cprMain.cprCount >= 100f)
                {
                    StartCoroutine(PlayFailureAudioThen(OnClear, clearClip));
                    
                }
                else
                {
                    if (failedCount == 0)
                    {
                        failedCount++;
                        StartCoroutine(PlayFailureAudioThen(OnFailureComplete, failAudioClip));
                    }
                    else
                    {
                        StartCoroutine(PlayFailureAudioThen(OnClear, completeClip));
                    }
                }
            }
        }

        // UI 업데이트
        cprCount.text = cprMain.cprCount.ToString("F0");
        timerText.text = timerSec.ToString();
    }


    public void TimerStart()
    {
        if (timerSec <= 0.01f)
        {
            timerSec = timerSecF1;
            cprMain.cprCount = 0;
        }

        isTimerRunning = true;
    }

    IEnumerator PlayFailureAudioThen(System.Action onComplete, AudioClip audioClip)
    {
        if (audioSource != null && audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
            yield return new WaitForSeconds(audioClip.length);
        }

        yield return new WaitForSeconds(1f);
        onComplete?.Invoke();
    }

    private void OnFailureComplete()
    {
        TimerStart();
    }

    private void OnClear()
    {
        myTR.TriggerStep(26);
    }
}
