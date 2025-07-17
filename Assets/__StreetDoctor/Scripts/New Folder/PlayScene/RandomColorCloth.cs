using UnityEngine;

public class RandomColorOnStart : MonoBehaviour
{
    void Start()
    {
        SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
        if (smr == null)
        {
            Debug.LogError("SkinnedMeshRenderer 없음");
            return;
        }

        // 원본 materials 배열 가져오기
        Material[] originalMats = smr.materials;

        // 복사할 머테리얼 배열 생성
        Material[] newMats = new Material[originalMats.Length];

        for (int i = 0; i < originalMats.Length; i++)
        {
            if (originalMats[i] == null) continue;

            // 개별 머테리얼 복제
            newMats[i] = new Material(originalMats[i]);

            // 랜덤 색상 생성
            Color randomColor = new Color(Random.value, Random.value, Random.value, 1f);

            // URP Lit Shader에서 base color는 "_BaseColor"
            newMats[i].SetColor("_BaseColor", randomColor);
        }

        // 복사된 머테리얼 배열을 적용
        smr.materials = newMats;
    }
}
