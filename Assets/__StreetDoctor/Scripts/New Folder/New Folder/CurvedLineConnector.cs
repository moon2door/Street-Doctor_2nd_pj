using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CurvedLineConnector : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public int segmentCount = 40;      // 선을 얼마나 세분화할지
    public int controlPoints = 5;      // 꺾이는 중간 포인트 개수
    public float curveAmplitude = 0.1f; // 곡선 세기

    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = segmentCount;
    }

    void Update()
    {
        if (startPoint == null || endPoint == null) return;

        Vector3[] points = new Vector3[controlPoints];
        Vector3 start = startPoint.position;
        Vector3 end = endPoint.position;

        // 중간 제어점 만들기 (X 방향 등간격 + Y축 노이즈)
        for (int i = 0; i < controlPoints; i++)
        {
            float t = i / (controlPoints - 1f);
            Vector3 pos = Vector3.Lerp(start, end, t);
            float offset = Mathf.Sin(t * Mathf.PI) * curveAmplitude;
            pos += Vector3.up * offset;
            points[i] = pos;
        }

        // Spline 구간 보간
        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (segmentCount - 1f);
            Vector3 p = CatmullRomSpline(points, t);
            lr.SetPosition(i, p);
        }
    }

    // Catmull-Rom 보간 함수
    Vector3 CatmullRomSpline(Vector3[] points, float t)
    {
        int count = points.Length;

        float scaledT = t * (count - 1);
        int i = Mathf.Clamp(Mathf.FloorToInt(scaledT), 1, count - 3);
        float localT = scaledT - i;

        Vector3 p0 = points[i - 1];
        Vector3 p1 = points[i];
        Vector3 p2 = points[i + 1];
        Vector3 p3 = points[i + 2];

        return 0.5f * (
            2f * p1 +
            (-p0 + p2) * localT +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * localT * localT +
            (-p0 + 3f * p1 - 3f * p2 + p3) * localT * localT * localT
        );
    }
}
