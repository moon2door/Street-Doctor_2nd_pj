using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class HandRayUI : MonoBehaviour
{
    [Header("설정")]
    public float rayLength = 5f;
    public LayerMask uiLayer;

    [Header("UI 히트 표시 이미지")]
    public GameObject indicatorObject;
    public Sprite indicatorSprite;

    [Header("외부 오브젝트 제어")]
    public GameObject handMesh;
    public GameObject oculusController;

    private LineRenderer lineRenderer;
    private bool isLeftHand = false;
    private bool isClick = false;

    public float rayLenth;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        if (gameObject.name.Contains("LeftHandAnchor"))
            isLeftHand = true;
        else if (gameObject.name.Contains("RightHandAnchor"))
            isLeftHand = false;

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;

        if (indicatorObject != null)
        {
            indicatorObject.SetActive(false);
            var img = indicatorObject.GetComponent<UnityEngine.UI.Image>();
            if (img != null && indicatorSprite != null)
                img.sprite = indicatorSprite;
        }
    }

    void Update()
    {
        if (handMesh != null && oculusController != null)
        {
            SkinnedMeshRenderer smr = handMesh.GetComponent<SkinnedMeshRenderer>();
            if (smr != null)
            {
                bool isVisible = smr.enabled;

                oculusController.SetActive(!isVisible);
                lineRenderer.enabled = !isVisible;
                if (indicatorObject != null)
                    indicatorObject.SetActive(!isVisible);

                if (isVisible)
                    return;
            }
        }

        Vector3 startPosition = transform.position + transform.forward * rayLenth;
        Ray ray = new Ray(startPosition, transform.forward);

        RaycastHit hit;
        Vector3 endPosition = ray.origin + ray.direction * rayLength;

        bool triggerPressed = isLeftHand
            ? OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)
            : OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger);

        if (Physics.Raycast(ray, out hit, rayLength, uiLayer))
        {
            endPosition = hit.point;

            if (indicatorObject != null)
            {
                indicatorObject.SetActive(true);
                indicatorObject.transform.position = hit.point + hit.normal * 0.01f;
                indicatorObject.transform.forward = -hit.normal;
            }

            if (triggerPressed && !isClick)
            {
                Debug.Log(isLeftHand ? "왼손 트리거 눌림" : "오른손 트리거 눌림");

                UICodeReceiver receiver = hit.collider.GetComponent<UICodeReceiver>();
                if (receiver != null)
                {
                    receiver.ExecuteCode();
                    StartCoroutine(ClickDelay());
                }
            }
        }
        else
        {
            if (indicatorObject != null)
                indicatorObject.SetActive(false);
        }

        lineRenderer.SetPosition(0, ray.origin);
        lineRenderer.SetPosition(1, endPosition);
    }

    IEnumerator ClickDelay()
    {
        isClick = true;
        yield return new WaitForSeconds(5f);
        isClick = false;
    }
}
