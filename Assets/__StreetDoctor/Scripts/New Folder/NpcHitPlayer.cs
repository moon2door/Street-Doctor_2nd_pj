using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcHitPlayer : MonoBehaviour
{
    public RandomPatrol patrol;

    [Header("사운드 참조")]
    public AudioSource myAudio;
    public AudioClip fallDownClip;
    public AudioClip peopleClip;

    [Header("사운드 딜레이")]
    public float fallDownDelay = 1.2f;


    private void Start()
    {
        myAudio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ✅ 매니저에게 먼저 허락 요청
            if (!NpcManager.Instance.TryTriggerNPC(patrol))
                return;

            // ✅ 직속 부모의 Collider 비활성화
            var colliders = transform.parent?.GetComponents<Collider>();
            if (colliders != null)
            {
                foreach (var col in colliders)
                    col.enabled = false;
            }

            patrol.StopAndRotate90();

            StartCoroutine(soundPlay());
        }
    }

    IEnumerator soundPlay()
    {
        // 첫 번째 사운드 (넘어지는 소리)
        yield return new WaitForSeconds(fallDownDelay);
        myAudio.PlayOneShot(fallDownClip);

        // 사람들 소리 재생 전 대기
        yield return new WaitForSeconds(2f);

        // 두 번째 사운드 (반복 재생 + 페이드 인)
        myAudio.clip = peopleClip;
        myAudio.volume = 0f; // 처음은 0
        myAudio.loop = true;
        myAudio.spatialBlend = 0;
        myAudio.Play();

        // 페이드 인 시작
        StartCoroutine(FadeInVolume(0.3f, 4f)); // 2초 동안 0.3까지 증가
    }

    IEnumerator FadeInVolume(float targetVolume, float duration)
    {
        float timer = 0f;
        float startVolume = myAudio.volume;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            myAudio.volume = Mathf.Lerp(startVolume, targetVolume, timer / duration);
            yield return null;
        }

        myAudio.volume = targetVolume; // 정확히 맞춰 마무리
    }

}
