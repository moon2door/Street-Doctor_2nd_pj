using UnityEngine;
using UnityEngine.UI;

public class MinimapIcon : MonoBehaviour
{
    public RectTransform minimapRect;
    public Transform playerTransform;
    public Vector2 mapWorldSize = new Vector2(100, 100); // 월드 크기

    void Update()
    {
        Vector3 pos = playerTransform.position;

        // 0~1 정규화
        float normalizedX = pos.x / mapWorldSize.x;
        float normalizedY = pos.z / mapWorldSize.y;

        // 미니맵 좌표로 변환
        float minimapX = (normalizedX - 0.5f) * minimapRect.sizeDelta.x;
        float minimapY = (normalizedY - 0.5f) * minimapRect.sizeDelta.y;

        // 현재 아이콘 위치 갱신
        GetComponent<RectTransform>().anchoredPosition = new Vector2(minimapX, minimapY);
    }
}
