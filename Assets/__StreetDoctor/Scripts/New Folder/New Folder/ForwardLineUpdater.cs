using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ForwardRayLine : MonoBehaviour
{
    public float lineLength = 2f;
    public Vector3 offset = Vector3.zero;

    [Header("참조 스크립트")]
    public LeftHandFistDetector fistDetector;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        if (!lineRenderer.enabled) return; // ✅ LineRenderer 꺼져 있으면 아무것도 하지 않음

        if (fistDetector == null)
        {
            fistDetector = FindObjectOfType<LeftHandFistDetector>();
            if (fistDetector == null) return;
        }

        UpdateLine();

        if (CheckRaycastHit(out RaycastHit hit))
        {
            if (fistDetector.IsLeftHandFist())
            {
                Debug.Log("🔍 라인에 오브젝트 감지됨 + 왼손 주먹");
            }
        }
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
