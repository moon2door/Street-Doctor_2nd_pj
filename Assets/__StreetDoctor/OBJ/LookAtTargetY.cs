using UnityEngine;

public class SmoothLookAtY : MonoBehaviour
{
    public Transform hingeTarget;         // 회전 중심 (힌지)
    public Transform objectA;             // 함께 회전할 오브젝트 A
    public Transform objectB;             // 함께 회전할 오브젝트 B
    public float rotationSpeed = 45f;     // 초당 회전 속도 (도)
    private float rotatedAngle = 0f;      // 현재까지 회전된 각도
    private float targetAngle = -90f;     // 최종 목표 각도

    private bool rotating = false;        // 처음엔 false

    void Update()
    {
        if (!rotating || hingeTarget == null || objectA == null || objectB == null) return;

        // 회전할 각도 계산
        float step = rotationSpeed * Time.deltaTime;

        // 목표 각도까지 남은 회전량
        float angleLeft = Mathf.Abs(targetAngle) - Mathf.Abs(rotatedAngle);

        if (step >= angleLeft)
        {
            step = angleLeft;
            rotating = false; // 회전 완료
        }

        float signedStep = step * Mathf.Sign(targetAngle);

        // 셋 다 힌지를 기준으로 회전
        transform.RotateAround(hingeTarget.position, Vector3.up, signedStep);
        objectA.RotateAround(hingeTarget.position, Vector3.up, signedStep);
        objectB.RotateAround(hingeTarget.position, Vector3.up, signedStep);

        rotatedAngle += signedStep;
    }

    // 손이 닿으면 회전 시작
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            rotating = true;
            rotatedAngle = 0f; // 회전 초기화 (원하면 유지해도 됨)
        }
    }
}
