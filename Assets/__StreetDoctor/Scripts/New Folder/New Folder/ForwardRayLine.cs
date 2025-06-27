using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ForwardRayLine : MonoBehaviour
{
    public float lineLength = 2f;
    public Vector3 offset = Vector3.zero;

    [Header("참조 스크립트")]
    public LeftHandFistDetector fistDetector;

    private LineRenderer lineRenderer;
    private GameObject currentOutlined;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        if (!lineRenderer.enabled)
        {
            if (currentOutlined != null)
            {
                ToggleOutline(currentOutlined, false);
                currentOutlined = null;
            }
            return;
        }

        if (fistDetector == null)
        {
            fistDetector = FindObjectOfType<LeftHandFistDetector>();
            if (fistDetector == null) return;
        }

        UpdateLine();

        if (CheckRaycastHit(out RaycastHit hit))
        {
            GameObject hitObj = hit.collider.gameObject;

            if (currentOutlined != hitObj)
            {
                // 이전 오브젝트의 아웃라인 끄기
                if (currentOutlined != null)
                    ToggleOutline(currentOutlined, false);

                // 새 오브젝트의 아웃라인 켜기
                ToggleOutline(hitObj, true);
                currentOutlined = hitObj;
            }
        }
        else
        {
            // 레이가 아무것도 안 맞았을 때 현재 아웃라인 제거
            if (currentOutlined != null)
            {
                ToggleOutline(currentOutlined, false);
                currentOutlined = null;
            }
        }
    }


    void ToggleOutline(GameObject obj, bool state)
    {
        var outline = obj.GetComponent<Outline>();
        if (outline != null)
            outline.enabled = state;
    }

    void UpdateLine()
    {
        Vector3 startPos = transform.position + offset;
        Vector3 endPos = startPos + transform.forward * lineLength;

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

    bool CheckRaycastHit(out RaycastHit hitInfo)
    {
        Vector3 start = transform.position + offset;
        Vector3 direction = transform.forward;

        return Physics.Raycast(start, direction, out hitInfo, lineLength);
    }
}
