using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TpScene : MonoBehaviour
{
    public string sceneName;
    public Image fadeImage; // 페이드용 이미지 (검정색, 전체 화면)
    public float fadeDuration = 1.0f;

    private bool isFading = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isFading)
        {
            StartCoroutine(FadeAndLoadScene());
        }
    }

    IEnumerator FadeAndLoadScene()
    {
        isFading = true;

        float timer = 0f;
        Color color = fadeImage.color;

        // 페이드 아웃 (투명 → 검정)
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        // 씬 로드
        SceneManager.LoadScene(sceneName);
    }
}
