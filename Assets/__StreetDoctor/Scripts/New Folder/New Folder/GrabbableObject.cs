using UnityEngine;

public static class TransformExtensions
{
    public static void SetParentKeepWorldScale(this Transform child, Transform newParent)
    {
        Vector3 worldScale = child.lossyScale;
        child.SetParent(newParent);
        Vector3 parentScale = newParent == null ? Vector3.one : newParent.lossyScale;

        child.localScale = new Vector3(
            worldScale.x / (parentScale.x == 0 ? 1 : parentScale.x),
            worldScale.y / (parentScale.y == 0 ? 1 : parentScale.y),
            worldScale.z / (parentScale.z == 0 ? 1 : parentScale.z)
        );
    }
}

public class GrabbableObject : MonoBehaviour
{
    [HideInInspector] public bool isGrabbed = false;

    [Header("릴리즈 시 붙을 부모")]
    public Transform returnParent;
    private Vector3 savedPosition;

    [Header("오브젝트 고유 ID")]
    public int objectID = 0;

    public bool isOK_CPR;

    [Header("머테리얼 색을 바꿀 오브젝트")]
    public GameObject targetObject;

    [Header("손에 붙을 오프셋 (옵션)")]
    public Vector3 grabOffset = Vector3.zero;
    public Vector3 grabRotationOffsetEuler = Vector3.zero;

    public Quaternion grabRotationOffset => Quaternion.Euler(grabRotationOffsetEuler);

    private Collider myCollider;
    private Material targetMaterial;
    private bool materialChanged = false;

    // 튜토리얼 큐브 전용 옵션
    [Header("튜토리얼 전용 옵션")]
    public bool isTutorialObject = false;
    private bool hasTriggered = false;
    private Vector3 tutorialSavedPosition;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
        savedPosition = transform.position;

        // 튜토리얼
        if (isTutorialObject)
            tutorialSavedPosition = transform.position;

        if (targetObject != null)
        {
            Renderer rend = targetObject.GetComponent<Renderer>();
            if (rend != null)
            {
                targetMaterial = rend.material;
                targetMaterial.color = new Color(1f, 0f, 0f, 0.35f); // 빨간색 반투명
            }
        }
    }

    private void Update()
    {
        if (!isGrabbed || targetObject == null || myCollider == null || targetMaterial == null) return;

        CodeID[] targets = GameObject.FindObjectsOfType<CodeID>();
        bool matched = false;

        foreach (var target in targets)
        {
            if (target.objectID == this.objectID)
            {
                Collider targetCol = target.GetComponent<Collider>();
                if (targetCol != null && myCollider.bounds.Intersects(targetCol.bounds))
                {
                    matched = true;
                    break;
                }
            }
        }

        if (matched)
        {
            if (!materialChanged)
            {
                targetMaterial.color = new Color(0f, 1f, 0f, 0.35f); // 초록색 반투명
                isOK_CPR = true;
            }
        }
        else
        {
            if (materialChanged)
            {
                targetMaterial.color = new Color(1f, 0f, 0f, 0.35f); // 빨간색 반투명
                isOK_CPR = false;
            }
        }
    }

    public void Grab(Transform handTransform)
    {
        isGrabbed = true;
        transform.SetParentKeepWorldScale(handTransform);
        transform.localPosition = grabOffset;
        transform.localRotation = grabRotationOffset;

        // 튜토리얼 
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;
    }

    public void Release()
    {
        isGrabbed = false;
        // 튜토리얼 
        if (isTutorialObject)
        {            
            transform.SetParent(null);              
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                var handRb = GetComponentInParent<Rigidbody>();
                if (handRb != null)
                {
                    rb.velocity = handRb.velocity;
                    rb.angularVelocity = handRb.angularVelocity;
                }
            }           
            return;
        }

        CodeID[] targets = GameObject.FindObjectsOfType<CodeID>();
        foreach (var target in targets)
        {
            if (target.objectID == this.objectID)
            {
                Collider targetCol = target.GetComponent<Collider>();
                if (targetCol != null && myCollider != null && myCollider.bounds.Intersects(targetCol.bounds))
                {
                    transform.SetParentKeepWorldScale(target.transform);
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;

                    isOK_CPR = true;
                    return;
                }
            }
        }

        if (returnParent != null)
        {
            transform.SetParentKeepWorldScale(returnParent);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            isOK_CPR = false;
        }
        else
        {
            transform.SetParentKeepWorldScale(null);
        }
    }
    //튜토리얼
    private void OnTriggerEnter(Collider other)
    {
        if (!isTutorialObject || hasTriggered) return;

        if (other.CompareTag("BasketZone"))
        {
            hasTriggered = true;
            Debug.Log("[GrabbableObject] 바구니 트리거 감지 → 카운트 후 리셋");
            if (CubeManager.Instance != null && CubeManager.Instance.basketSFX != null)
                AudioSource.PlayClipAtPoint(CubeManager.Instance.basketSFX, transform.position);
            CubeManager.Instance.OnCubeEnteredBasket(this);
        }
        else if (other.CompareTag("GroundZone"))
        {
            Debug.Log("[GrabbableObject] 바닥 트리거 감지 → 원위치 복귀");
            ReturnToTutorialPosition();          
        }
    }
    //튜토리얼
    void ReturnToTutorialPosition()
    {
        transform.SetParent(null);
        transform.position = tutorialSavedPosition;
        transform.rotation = Quaternion.identity;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        hasTriggered = false;
    }

    public void ResetState()
    {
        savedPosition = transform.position;
        materialChanged = false;

        if (targetMaterial != null)
        {
            targetMaterial.color = new Color(1f, 0f, 0f, 0.35f); // 초기 상태로 되돌림
        }
        if (isTutorialObject)
        {
            tutorialSavedPosition = transform.position;
            hasTriggered = false;
        }
    }
}
