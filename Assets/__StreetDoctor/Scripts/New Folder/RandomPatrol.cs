using UnityEngine;
using UnityEngine.AI;

public class RandomPatrol : MonoBehaviour
{
    [Header("이동 설정")]
    public float minPatrolDistance = 3f; // ✅ 최소 이동 거리
    public float patrolRadius = 10f;     // ✅ 최대 이동 반경

    [Header("대기 시간 설정")]
    public float waitTimeMin = 2f;       // ✅ 최소 대기 시간
    public float waitTimeMax = 5f;       // ✅ 최대 대기 시간
    private float currentWaitTime;       // 랜덤으로 정해질 현재 대기 시간

    private NavMeshAgent agent;
    private Animator anim;

    private float waitTimer;
    private bool waiting;
    private bool isStopped = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        MoveToRandomPoint();
    }

    void Update()
    {
        if (isStopped) return;

        if (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
        {
            anim.SetBool("Walk_B", true);
        }
        else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            anim.SetBool("Walk_B", false);

            if (!waiting)
            {
                waitTimer = 0f;
                waiting = true;
                currentWaitTime = Random.Range(waitTimeMin, waitTimeMax); // ✅ 대기 시간 랜덤 설정
            }

            waitTimer += Time.deltaTime;

            if (waitTimer >= currentWaitTime)
            {
                MoveToRandomPoint();
                waiting = false;
            }
        }
    }

    void MoveToRandomPoint()
    {
        Vector3 randomDirection;
        float distance;

        // ✅ 최소~최대 거리 범위 내 랜덤 거리 설정
        do
        {
            randomDirection = Random.insideUnitSphere * patrolRadius;
            randomDirection.y = 0;
            distance = randomDirection.magnitude;
        } while (distance < minPatrolDistance);

        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    public void StopAndRotate90()
    {
        isStopped = true;
        agent.isStopped = true;

        anim.SetBool("Walk_B", false);
        anim.SetTrigger("Dying_T");

        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + 90f, 0);
    }
}
