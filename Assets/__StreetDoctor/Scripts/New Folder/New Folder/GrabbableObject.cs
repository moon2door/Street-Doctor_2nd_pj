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

    [Header("오브젝트 고유 ID")]
    public int objectID = 0;

    [Header("머테리얼 색을 바꿀 오브젝트")]
    public GameObject targetObject;

    [Header("손에 붙을 오프셋 (옵션)")]
    public Vector3 grabOffset = Vector3.zero;
    public Vector3 grabRotationOffsetEuler = Vector3.zero;

    public Quaternion grabRotationOffset => Quaternion.Euler(grabRotationOffsetEuler);

    private Collider myCollider;
    private Material targetMaterial;
    private bool materialChanged = false;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();

        if (targetObject != null)
        {
            Renderer rend = targetObject.GetComponent<Renderer>();
            if (rend != null)
            {
                targetMaterial = rend.material;
                // 처음에는 붉은색 반투명으로 초기화
                targetMaterial.color = new Color(1f, 0f, 0f, 0.35f);
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
                materialChanged = true;
            }
        }
        else
        {
            if (materialChanged)
            {
                targetMaterial.color = new Color(1f, 0f, 0f, 0.35f); // 빨간색 반투명
                materialChanged = false;
            }
        }
    }

    public void Grab(Transform handTransform)
    {
        isGrabbed = true;
        transform.SetParentKeepWorldScale(handTransform);
        transform.localPosition = grabOffset;
        transform.localRotation = grabRotationOffset;
    }

    public void Release()
    {
        isGrabbed = false;

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
                    return;
                }
            }
        }

        if (returnParent != null)
        {
            transform.SetParentKeepWorldScale(returnParent);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            transform.SetParentKeepWorldScale(null);
        }
    }
}
