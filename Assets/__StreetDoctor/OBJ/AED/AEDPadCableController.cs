using UnityEngine;

public class AEDPadCableController : MonoBehaviour
{
    [Header("패드와 케이블 위치")]
    public Transform pad;             // Pad1 또는 Pad2
    public Transform cableStart;      // 케이블 시작점 (예: Plug)

    [Header("Bone 순서대로 할당")]
    public Transform[] bones;         // bone.01 ~ Bone.L.pad1_end

    [Header("Bezier 곡선 제어")]
    public float curveHeight = 0.2f;  // 곡선 휘어짐 높이 (상황에 맞게 조절)

    void Update()
    {
        UpdateCableBones();
    }

    void UpdateCableBones()
    {
        Vector3 start = cableStart.position;
        Vector3 end = pad.position;

        // 제어 포인트 설정: 중간점에서 위로 들어올림
        Vector3 control = (start + end) / 2 + Vector3.up * curveHeight;

        for (int i = 0; i < bones.Length; i++)
        {
            float t = (float)i / (bones.Length - 1);

            // Bezier 곡선을 따라 위치 계산
            Vector3 targetPos = CalculateQuadraticBezierPoint(start, control, end, t);
            bones[i].position = targetPos;

            // 방향 지정 (끝 bone은 회전 생략)
            if (i < bones.Length - 1)
            {
                Vector3 dir = CalculateQuadraticBezierPoint(start, control, end, t + 0.01f) - targetPos;
                if (dir != Vector3.zero)
                    bones[i].rotation = Quaternion.LookRotation(dir);
            }
        }
    }

    // 2차 베지어 곡선 계산 함수
    Vector3 CalculateQuadraticBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        Vector3 a = Vector3.Lerp(p0, p1, t);
        Vector3 b = Vector3.Lerp(p1, p2, t);
        return Vector3.Lerp(a, b, t);
    }
}
