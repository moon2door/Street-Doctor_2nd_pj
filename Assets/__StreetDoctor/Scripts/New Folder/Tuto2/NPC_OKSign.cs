using UnityEngine;

public class NPC_OKSign : MonoBehaviour
{
    public bool isBlinking = false;

    void Update()
    {
        // 매 프레임마다 아웃라인 상태를 확인해서 갱신
        var outline = GetComponent<Outline>();
        if (outline != null)
        {
            isBlinking = outline.enabled;  // 깜빡이는 순간만 true면 더 디테일하게 조절 가능
        }
    }
}
