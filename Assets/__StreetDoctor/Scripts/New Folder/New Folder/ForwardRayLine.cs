using UnityEngine;
using UnityEngine.UI;

public class ForwardRayLine : MonoBehaviour
{
    public float lineLength = 2f;
    public Vector3 offset = Vector3.zero;
    public LayerMask npcLayerMask;

    [Header("참조 스크립트")]
    public LeftHandFistDetector fistDetector;

    [HideInInspector] public bool isBlink;
    [HideInInspector] public NPC_OKSign currentTarget = null; // ✅ 현재 감지된 NPC

    private GameObject currentOutlined;
    private GameObject lastHitObj = null;

    private Image currentProgressImage;

    private float hoverTime = 0f;
    private bool blinking = false;
    private float blinkTimer = 0f;
    private float blinkInterval = 0.5f;

    private float maxHoverTime = 2f;

    // ⏱️ 시작 후 무시 시간
    private float startupTime = 1f;
    private float elapsedTime = 0f;

    void Update()
    {
        if (fistDetector == null)
        {
            fistDetector = FindObjectOfType<LeftHandFistDetector>();
            if (fistDetector == null) return;
        }

        elapsedTime += Time.deltaTime;
        if (elapsedTime < startupTime)
        {
            StopBlinking();
            return;
        }

        Vector3 start = transform.position + offset;
        Vector3 direction = transform.forward;
        Debug.DrawRay(start, direction * lineLength, Color.red);

        if (Physics.Raycast(start, direction, out RaycastHit hit, lineLength, npcLayerMask, QueryTriggerInteraction.Collide))
        {
            GameObject hitObj = hit.collider.gameObject;

            //Debug.LogError($"Ray가 닿은 오브젝트: {hitObj.name}");

            // 🛑 Outline 없으면 무시
            if (hitObj.GetComponent<Outline>() == null)
            {
                StopBlinking();
                ToggleOutline(currentOutlined, false);
                currentOutlined = null;
                lastHitObj = null;
                currentProgressImage = null;
                currentTarget = null; // ✅ 감지된 NPC 초기화
                return;
            }

            if (currentOutlined != hitObj || lastHitObj != hitObj)
            {
                StopBlinking();
                ToggleOutline(currentOutlined, false);

                currentOutlined = hitObj;
                ToggleOutline(currentOutlined, true);
                hoverTime = 0f;

                // 하위까지 포함해서 찾기
                Transform fillTransform = FindInChildren(hitObj.transform, "fillProgressImage");
                currentProgressImage = fillTransform != null ? fillTransform.GetComponent<Image>() : null;

                if (currentProgressImage != null)
                    currentProgressImage.fillAmount = 0f;
            }
            else
            {
                hoverTime += Time.deltaTime;

                float fillProgress = Mathf.Clamp01(hoverTime / maxHoverTime);
                if (currentProgressImage != null)
                    currentProgressImage.fillAmount = fillProgress;

                if (hoverTime >= maxHoverTime && !blinking)
                {
                    blinking = true;
                    isBlink = true;

                    TrainingEvaluator.Instance.didCallHelp = true;
                    blinkTimer = 0f;

                    // ✅ 감지된 NPC 저장
                    currentTarget = hitObj.GetComponent<NPC_OKSign>();
                }

                if (blinking)
                {
                    blinkTimer += Time.deltaTime;
                    if (blinkTimer >= blinkInterval)
                    {
                        blinkTimer = 0f;
                        ToggleOutline(currentOutlined, !IsOutlineEnabled(currentOutlined));
                    }
                }
            }

            lastHitObj = hitObj;
        }
        else
        {
            StopBlinking();
            ToggleOutline(currentOutlined, false);
            currentOutlined = null;
            lastHitObj = null;
            currentProgressImage = null;
            currentTarget = null; // ✅ 레이가 닿지 않으면 초기화
        }
    }

    void StopBlinking()
    {
        blinking = false;
        isBlink = false;
        hoverTime = 0f;
        blinkTimer = 0f;
        currentTarget = null;

        if (currentProgressImage != null)
            currentProgressImage.fillAmount = 0f;
    }

    void ToggleOutline(GameObject obj, bool state)
    {
        if (obj == null) return;
        var outline = obj.GetComponent<Outline>();
        if (outline != null)
            outline.enabled = state;
    }

    bool IsOutlineEnabled(GameObject obj)
    {
        if (obj == null) return false;
        var outline = obj.GetComponent<Outline>();
        return outline != null && outline.enabled;
    }

    // ✅ 하위 자식까지 포함해서 특정 이름의 Transform 찾기
    Transform FindInChildren(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform result = FindInChildren(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
}
