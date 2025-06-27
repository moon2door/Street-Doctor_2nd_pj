using UnityEngine;

public class ForwardRayLine : MonoBehaviour
{
    public float lineLength = 2f;
    public Vector3 offset = Vector3.zero;

    [Header("참조 스크립트")]
    public LeftHandFistDetector fistDetector;

    private GameObject currentOutlined;
    private float hoverTime = 0f;
    private bool blinking = false;
    private float blinkTimer = 0f;
    private float blinkInterval = 0.5f;

    void Update()
    {
        if (fistDetector == null)
        {
            fistDetector = FindObjectOfType<LeftHandFistDetector>();
            if (fistDetector == null) return;
        }

        Vector3 start = transform.position + offset;
        Vector3 direction = transform.forward;
        Debug.DrawRay(start, direction * lineLength, Color.red);

        if (Physics.Raycast(start, direction, out RaycastHit hit, lineLength))
        {
            GameObject hitObj = hit.collider.gameObject;

            if (currentOutlined != hitObj)
            {
                StopBlinking(); // 기존 아웃라인 초기화
                ToggleOutline(currentOutlined, false);

                currentOutlined = hitObj;
                ToggleOutline(currentOutlined, true);
                hoverTime = 0f;
            }
            else
            {
                hoverTime += Time.deltaTime;

                if (hoverTime >= 2f && !blinking)
                {
                    blinking = true;
                    blinkTimer = 0f;
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
        }
        else
        {
            StopBlinking();
            ToggleOutline(currentOutlined, false);
            currentOutlined = null;
        }
    }

    void StopBlinking()
    {
        blinking = false;
        hoverTime = 0f;
        blinkTimer = 0f;
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
}
