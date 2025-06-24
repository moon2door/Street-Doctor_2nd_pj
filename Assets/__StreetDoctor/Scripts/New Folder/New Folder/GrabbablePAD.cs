using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbablePAD : MonoBehaviour
{
    [HideInInspector] public bool isGrabbed = false;

    [Header("릴리즈 시 붙을 부모")]
    public Transform returnParent;

    [Header("오브젝트 고유 ID")]
    public int objectID = 0;

    [Header("머테리얼 색을 바꿀 오브젝트")]
    public GameObject targetObject;

    private Collider myCollider;
    private Material targetMaterial;
    private bool materialChanged = false;

    private Transform handTransform; // 손 Transform을 저장

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
        // 손을 따라 움직임
        if (isGrabbed && handTransform != null)
        {
            transform.position = handTransform.position;
            transform.rotation = handTransform.rotation;
        }

        // 패드의 위치가 정답 위치에 맞았는지 확인
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

    public void Grab(Transform hand)
    {
        isGrabbed = true;
        handTransform = hand;
    }

    public void Release()
    {
        isGrabbed = false;
        handTransform = null;

        CodeID[] targets = GameObject.FindObjectsOfType<CodeID>();
        foreach (var target in targets)
        {
            if (target.objectID == this.objectID)
            {
                Collider targetCol = target.GetComponent<Collider>();
                if (targetCol != null && myCollider != null && myCollider.bounds.Intersects(targetCol.bounds))
                {
                    transform.SetParent(target.transform);
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                    return;
                }
            }
        }

        if (returnParent != null)
        {
            transform.SetParent(returnParent);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            transform.SetParent(null);
        }
    }
}
