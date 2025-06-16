using UnityEngine;
using UnityEngine.AI;

public class RandomPatrol : MonoBehaviour
{
    public float patrolRadius = 10f;
    public float waitTime = 2f;

    private NavMeshAgent agent;
    private float waitTimer;
    private bool waiting;
    private bool isStopped = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        MoveToRandomPoint();
    }

    void Update()
    {
        if (isStopped) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!waiting)
            {
                waitTimer = 0f;
                waiting = true;
            }

            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                MoveToRandomPoint();
                waiting = false;
            }
        }
    }

    void MoveToRandomPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection.y = 0; // Y축 고정
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

        // 현 방향에서 Y축으로 90도 회전
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + 90f, 0);
    }
}
