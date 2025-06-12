using UnityEngine;
using UnityEngine.UI;

public class MinimapIcon : MonoBehaviour
{
    public RectTransform minimapRect;
    public Transform playerTransform;
    public Vector2 mapWorldSize = new Vector2(100, 100); // ���� ũ��

    void Update()
    {
        Vector3 pos = playerTransform.position;

        // 0~1 ����ȭ
        float normalizedX = pos.x / mapWorldSize.x;
        float normalizedY = pos.z / mapWorldSize.y;

        // �̴ϸ� ��ǥ�� ��ȯ
        float minimapX = (normalizedX - 0.5f) * minimapRect.sizeDelta.x;
        float minimapY = (normalizedY - 0.5f) * minimapRect.sizeDelta.y;

        // ���� ������ ��ġ ����
        GetComponent<RectTransform>().anchoredPosition = new Vector2(minimapX, minimapY);
    }
}
