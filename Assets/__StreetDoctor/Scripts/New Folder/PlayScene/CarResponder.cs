using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class CarResponder : MonoBehaviour
{
    private NavMeshAgent agent;
    private bool hasMoved = false;

    [Header("트리거 설정")]
    public bool isTriggered = false;

    [Header("목표 대상 (예: 플레이어)")]
    public Transform target;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!hasMoved && isTriggered && target != null && agent.isOnNavMesh)
        {
            MoveToClosestPointNearTarget();
            hasMoved = true;
        }

        // ❗ 도착 전 일정 거리 도달 시 멈춤
        if (hasMoved && !agent.pathPending && agent.remainingDistance > 0 && agent.remainingDistance <= 2f)
        {
            agent.isStopped = true;
        }
    }



    void MoveToClosestPointNearTarget()
    {
        Vector3 dirToCar = (transform.position - target.position).normalized;
        Vector3 desiredPoint = target.position + dirToCar * 5f;

        // 주변 NavMesh에 있는 포인트를 샘플링
        if (NavMesh.SamplePosition(desiredPoint, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }


    // 외부에서 트리거를 호출하는 함수 (선택사항)
    public void ActivateResponse(Transform newTarget)
    {
        target = newTarget;
        isTriggered = true;
    }
}
