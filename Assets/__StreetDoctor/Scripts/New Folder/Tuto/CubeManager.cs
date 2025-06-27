using System.Collections;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    public static CubeManager Instance;
    
    [Header("큐브 관련 설정")]
    public GameObject cube;
    public Transform spawnPoint;
    public GameObject basket;
    
    [Header("목표 설정")]
    public int goal = 3;

    private int cubesCompleted = 0;
    private bool isCompleted = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    public void Start()
    {
        if (cube == null)
            cube = GameObject.Find("Cube");

        if (spawnPoint == null)
            spawnPoint = GameObject.Find("Obj _ Transform")?.transform;

        if (basket == null)
            basket = GameObject.Find("Basket");
    }
    public void StartCubeTask()
    {
        basket.SetActive(true);
        spawnPoint.gameObject.SetActive(true);
        SpawnCube();
    }

    public void OnCubeEnteredBasket(GrabbableObject cube)
    {
        if (isCompleted) return;

        cubesCompleted++;
        Debug.Log($"[CubeManager] 바구니 담김: {cube.gameObject.name}, 총 개수: {cubesCompleted}/{goal}");
        TutorialManager.Instance.UpdateCubeUI(cubesCompleted, goal);

        if (cubesCompleted >= goal)
        {
            isCompleted = true;
            TutorialManager.Instance.OnCubeMissionComplete();

            // 마지막 큐브 위치 초기화 시도
            if (cube != null)
            {
                GrabbableObject go = cube.GetComponent<GrabbableObject>();
                if (go != null) go.ResetState(); // 위치 리셋
            }
            //  바구니는 조금 뒤에 꺼지게 딜레이 처리
            StartCoroutine(DisableObjectsWithDelay());
            return;
        }
        SpawnCube();
    }
    private void SpawnCube()
    {
        if (cube == null || spawnPoint == null) return;

        cube.transform.position = spawnPoint.position;
        cube.transform.rotation = spawnPoint.rotation;

        Rigidbody rb = cube.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        GrabbableObject grabbable = cube.GetComponent<GrabbableObject>();
        if (grabbable != null)
        {
            grabbable.ResetState();
        }
        cube.SetActive(true);
    }
    IEnumerator DisableObjectsWithDelay()
    {
        yield return new WaitForSeconds(0.5f); // 큐브 리셋 시간 확보
        basket.SetActive(false);
        spawnPoint.gameObject.SetActive(false);
        if (cube != null)
        {
            Destroy(this.gameObject);
        }
    }
    public bool IsCompleted => isCompleted;
}
