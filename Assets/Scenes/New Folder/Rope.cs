using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Rope : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();

    [Header("줄 양 끝을 잇는 오브젝트")]
    public Transform startObject;
    public Transform endObject;

    [Header("줄 설정")]
    [Tooltip("줄 굵기")]
    public float lineWidth = 0.1f;

    [Tooltip("1미터 당 세그먼트 수 (민감도 조정용)")]
    public float segmentPerMeter = 15f;

    [Tooltip("세그먼트 수 최소값")]
    public int minSegment = 5;

    [Tooltip("세그먼트 수 최대값")]
    public int maxSegment = 70;

    private int segmentLength = 0;
    private float ropeSegLen = 0f;
    private float prevDistance = -1f;

    [Header("탄성 관련")]
    [Range(0f, 1f)]
    public float damping = 0.98f;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        UpdateRopeStructure();
    }

    void Update()
    {
        float currentDistance = Vector3.Distance(startObject.position, endObject.position);
        if (Mathf.Abs(currentDistance - prevDistance) > 0.01f)
        {
            UpdateRopeStructure();
        }

        DrawRope();
    }

    void FixedUpdate()
    {
        Simulate();
    }

    void UpdateRopeStructure()
    {
        Vector3 startPos = startObject.position;
        Vector3 endPos = endObject.position;
        float totalLength = Vector3.Distance(startPos, endObject.position);
        prevDistance = totalLength;

        int newSegmentLength = Mathf.Clamp(Mathf.RoundToInt(totalLength * segmentPerMeter), minSegment, maxSegment);

        if (newSegmentLength != segmentLength)
        {
            segmentLength = newSegmentLength;
            ropeSegLen = totalLength / (segmentLength - 1);

            Vector3 direction = (endPos - startPos).normalized;
            Vector3 ropePos = startPos;

            // 🔄 완전 초기화
            ropeSegments = new List<RopeSegment>(segmentLength);
            for (int i = 0; i < segmentLength; i++)
            {
                ropeSegments.Add(new RopeSegment(ropePos));
                ropePos += direction * ropeSegLen;
            }

            lineRenderer.positionCount = segmentLength;
        }
    }


    void Simulate()
    {
        Vector3 gravity = new Vector3(0f, -0.5f, 0f);

        for (int i = 0; i < segmentLength; i++)
        {
            RopeSegment seg = ropeSegments[i];
            Vector3 velocity = (seg.posNow - seg.posOld) * damping;
            seg.posOld = seg.posNow;
            seg.posNow += velocity;
            seg.posNow += gravity * Time.deltaTime;
            ropeSegments[i] = seg;
        }

        for (int i = 0; i < 100; i++)
        {
            ApplyConstraint();
        }
    }

    void ApplyConstraint()
    {
        ropeSegments[0] = new RopeSegment(startObject.position);
        ropeSegments[segmentLength - 1] = new RopeSegment(endObject.position);

        for (int i = 0; i < segmentLength - 1; i++)
        {
            RopeSegment segA = ropeSegments[i];
            RopeSegment segB = ropeSegments[i + 1];

            float dist = (segA.posNow - segB.posNow).magnitude;
            float error = dist - ropeSegLen;
            Vector3 changeDir = (segA.posNow - segB.posNow).normalized;
            Vector3 changeAmount = changeDir * error;

            if (i != 0)
            {
                segA.posNow -= changeAmount * 0.5f;
                segB.posNow += changeAmount * 0.5f;
                ropeSegments[i] = segA;
                ropeSegments[i + 1] = segB;
            }
            else
            {
                segB.posNow += changeAmount;
                ropeSegments[i + 1] = segB;
            }
        }
    }

    void DrawRope()
    {
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        for (int i = 0; i < segmentLength; i++)
        {
            lineRenderer.SetPosition(i, ropeSegments[i].posNow);
        }
    }

    public struct RopeSegment
    {
        public Vector3 posNow;
        public Vector3 posOld;

        public RopeSegment(Vector3 pos)
        {
            posNow = pos;
            posOld = pos;
        }
    }
}
