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

    private bool isReacting = false;
    private Vector3 reactTargetPosition;
    public float lookStopDistance = 4f; // 반응 후 멈출 거리

    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    public float stuckThresholdTime = 3f;       // 💡 3초 이상 가만히 있으면
    public float stuckMoveThreshold = 0.05f;    // 💡 거의 움직이지 않을 경우 거리

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        // 🔹 매니저에 자신 등록
        NpcManager.Instance?.RegisterNPC(this);

        MoveToRandomPoint();
    }

    void Update()
    {
        if (isStopped) return;

        // ✅ 반응 상태일 때 처리
        if (isReacting)
        {
            float distToTarget = Vector3.Distance(transform.position, reactTargetPosition);
            if (distToTarget <= lookStopDistance)
            {
                agent.isStopped = true;
                anim.SetBool("Walk_B", false);

                anim.applyRootMotion = true;

                // 천천히 쓰러진 NPC를 바라보게 회전
                Vector3 dir = (reactTargetPosition - transform.position).normalized;
                dir.y = 0;
                if (dir != Vector3.zero)
                {
                    Quaternion lookRot = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 2f);
                }

                return;
            }
        }

        // ✅ 이동 중인데도 제자리에서 머무는지 체크
        if (agent.velocity.magnitude > 0.1f) // 이동 중이면 위치 갱신
        {
            stuckTimer = 0f;
            lastPosition = transform.position;
        }
        else
        {
            float dist = Vector3.Distance(transform.position, lastPosition);
            if (dist < stuckMoveThreshold)
            {
                stuckTimer += Time.deltaTime;
                if (stuckTimer > stuckThresholdTime)
                {
                    Debug.LogWarning("🔁 NPC stuck detected, force moving to new point.");
                    MoveToRandomPoint();
                    stuckTimer = 0f;
                }
            }
            else
            {
                stuckTimer = 0f;
                lastPosition = transform.position;
            }
        }

        // ✅ 일반 순찰 처리
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
                currentWaitTime = Random.Range(waitTimeMin, waitTimeMax);
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

    public void ReactToFallenNPC(Vector3 targetPos)
    {
        isStopped = false;
        isReacting = true;
        reactTargetPosition = targetPos;

        agent.isStopped = false;
        anim.SetBool("Walk_B", true);
        agent.SetDestination(targetPos);
    }

}
