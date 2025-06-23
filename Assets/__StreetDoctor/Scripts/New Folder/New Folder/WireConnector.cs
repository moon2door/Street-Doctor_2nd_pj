using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WireConnector : MonoBehaviour
{
    public Transform fixedPoint;    // 오브젝트 B (Y자 중심)
    public Transform movingPoint;   // 오브젝트 A (AED 패드)

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        if (fixedPoint == null || movingPoint == null) return;

        lineRenderer.SetPosition(0, fixedPoint.position);
        lineRenderer.SetPosition(1, movingPoint.position);
    }
}
