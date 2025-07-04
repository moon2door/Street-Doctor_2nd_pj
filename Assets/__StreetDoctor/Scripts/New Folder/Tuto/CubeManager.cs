using System.Collections;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    public static CubeManager Instance;
    
    [Header("큐브 관련 설정")]
    public GameObject cube;
    public Transform spawnPoint;
    public GameObject basket;

    [Header("공용 효과음")]
    public AudioClip basketSFX;

    [Header("목표 설정")]
    public int goal = 3;

    private int cubesCompleted = 0;
    private bool isCompleted = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }    
    public void Start()
    {
        
    }
    public void StartCubeTask()
    {
        if (spawnPoint == null)
        {
            GameObject sp = GameObject.Find("Obj _ Transform");
            if (sp != null)
            {
                spawnPoint = sp.transform;
                Debug.Log("✅ spawnPoint 자동 연결됨");
            }
            else
            {
                Debug.LogError("❌ spawnPoint(Obj _ Transform)를 찾을 수 없습니다.");
                return;
            }
        }

        if (cube == null && spawnPoint != null)
        {
            Transform child = spawnPoint.Find("Cube");
            if (child != null)
            {
                cube = child.gameObject;
                Debug.Log("✅ Cube 자동 연결됨");
            }
            else
            {
                Debug.LogError("❌ spawnPoint 하위에 Cube 오브젝트가 없습니다.");
                return;
            }
        }

        if (basket == null)
        {
            GameObject b = GameObject.Find("Basket");
            if (b != null)
            {
                basket = b;
                Debug.Log("✅ Basket 자동 연결됨");
            }
            else
            {
                Debug.LogError("❌ Basket 오브젝트를 찾을 수 없습니다.");
                return;
            }
        }

        // 오브젝트 활성화
        cube.SetActive(true);
        spawnPoint.gameObject.SetActive(true);
        basket.SetActive(true);

        // 튜토리얼 전용 설정
        var grabbable = cube.GetComponent<GrabbableObject>();
        if (grabbable != null) grabbable.isTutorialObject = true;

        // 중력 초기화
        Rigidbody rb = cube.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;  // 손에서 잡히기 전까진 고정
        }

        grabbable?.ResetState();
    }

    public void OnCubeEnteredBasket(GrabbableObject cube)
    {
        if (isCompleted) return;

        cubesCompleted++;
        Debug.Log($"✅ 바스켓에 큐브 들어감 ({cubesCompleted}/{goal})");

        TutorialManager.Instance?.UpdateCubeUI(cubesCompleted, goal);

        if (cubesCompleted >= goal)
        {
            isCompleted = true;
            TutorialManager.Instance?.OnCubeMissionComplete();
            StartCoroutine(EndTaskDelay());
        }
        else
        {
            StartCoroutine(ResetCubeAfterDelay());
        }
    }

    IEnumerator ResetCubeAfterDelay()
    {
        yield return new WaitForSeconds(0.8f);

        cube.transform.position = spawnPoint.position;
        cube.transform.rotation = spawnPoint.rotation;

        Rigidbody rb = cube.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        cube.GetComponent<GrabbableObject>()?.ResetState();
    }

    IEnumerator EndTaskDelay()
    {
        yield return new WaitForSeconds(1f);

        cube.SetActive(false);
        basket.SetActive(false);
        spawnPoint.gameObject.SetActive(false);

        Debug.Log(" 큐브 미션 완료");
    }
}
