using UnityEngine;
using UnityEngine.AI;

public class RandomPrefabSpawner : MonoBehaviour
{
    public GameObject[] prefabs; // 프리팹 3개
    public int spawnCount;  // 생성할 총 개수
    public Vector3 center; // 배치 중심
    public float range; // 배치 범위 반지름

    void Start()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomPos = GetRandomNavMeshPosition(center, range);
            if (randomPos != Vector3.zero)
            {
                int randomIndex = Random.Range(0, prefabs.Length);
                Instantiate(prefabs[randomIndex], randomPos, Quaternion.identity);
            }
        }
    }

    Vector3 GetRandomNavMeshPosition(Vector3 center, float range)
    {
        for (int attempt = 0; attempt < spawnCount; attempt++) // 10번 시도
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return Vector3.zero; // 실패 시 (안정성 확보용)
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(center, range);
    }

}
