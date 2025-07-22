using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UICodeReceiver : MonoBehaviour
{
    public string sceneName;
    public int codeID;
    public AudioSource audioSource;
    public AudioClip audioClip;

    [Header("페이드 설정")]
    public Image fadeImage;           // 검정 이미지 오브젝트 (Image 컴포넌트)
    public float fadeDuration = 1f;   // 페이드 시간 (초 단위)

    public void ExecuteCode()
    {
        switch (codeID)
        {
            case 1:
            case 2:
            case 3:
            case 5:
                audioSource.PlayOneShot(audioClip);
                StartCoroutine(FadeAndLoad(sceneName));
                break;

            case 4:
                Application.Quit();
                break;

            default:
                Debug.Log("코드 설정 안됨");
                break;
        }
    }

    private IEnumerator FadeAndLoad(string targetScene)
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);

            Color color = fadeImage.color;
            float timer = 0f;

            while (timer < fadeDuration)
            {
                float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
                fadeImage.color = new Color(color.r, color.g, color.b, alpha);
                timer += Time.deltaTime;
                yield return null;
            }

            // 최종 알파 고정
            fadeImage.color = new Color(color.r, color.g, color.b, 1f);
        }

        SceneManager.LoadScene(targetScene);
    }
}
