using UnityEngine;
using UnityEngine.UI;

public class FingerTrigger : MonoBehaviour
{
    HandGrabber grabber;
    bool isLeftHand;
    CPRTraningStart cprTS;

    private float lastInputTime = 0f;

    [Header("터치 시 숫자 표시할 텍스트")]
    public Text phoneText;

    // 👉 모든 인스턴스에서 공유되는 카운터
    private static int phoneTextFoundCount = 0;

    void Start()
    {
        // CPR 트리거 가져오기
        cprTS = GameObject.Find("Start Zone")?.GetComponent<CPRTraningStart>();

        // 오른손만 탐색 및 카운트 증가
        if (!isLeftHand)
        {
            phoneText = GameObject.Find("Phone_T")?.GetComponent<Text>();

            if (phoneText != null)
            {
                phoneTextFoundCount++;
                Debug.Log($"[FingerTrigger] phoneTextFoundCount: {phoneTextFoundCount}");

                if (phoneTextFoundCount >= 2)
                {
                    GameObject phoneObj = GameObject.Find("Phone");
                    if (phoneObj != null)
                    {
                        phoneObj.SetActive(false);
                        Debug.Log("[FingerTrigger] Phone 오브젝트 비활성화됨");
                    }
                }
            }
        }
    }

    public void Setup(HandGrabber grabber, bool isLeft)
    {
        this.grabber = grabber;
        this.isLeftHand = isLeft;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grabbable"))
        {
            GrabbableObject obj = other.GetComponent<GrabbableObject>();
            if (obj != null)
                grabber.SetTarget(obj, isLeftHand);
        }

        if (isLeftHand) return;
        if (phoneText == null) return;

        if (Time.time - lastInputTime < 0.5f) return;
        lastInputTime = Time.time;

        string tag = other.tag;

        if (tag.StartsWith("Key"))
        {
            string keyVal = tag.Replace("Key", "");
            if (keyVal.Length == 1 && "0123456789*#".Contains(keyVal))
            {
                phoneText.text += keyVal;
            }
        }
        else if (tag == "DelButton")
        {
            if (phoneText.text.Length > 0)
            {
                phoneText.text = phoneText.text.Substring(0, phoneText.text.Length - 1);
            }
        }
        if (tag == "CallButton")
        {
            if (phoneText.text == "119")
            {
                cprTS.TriggerStep(12);
            }
            else
            {
                phoneText.text = "";
            }
        }
        //긴급통화 버튼
        else if (tag == "EmergencyButton")
        {
            phoneText.text = "119";
            cprTS.TriggerStep(12);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Grabbable"))
        {
            GrabbableObject obj = other.GetComponent<GrabbableObject>();
            if (obj != null)
            {
                Transform currentHand = isLeftHand ? grabber.leftHandTransform : grabber.rightHandTransform;
                if (obj.transform.parent != currentHand)
                {
                    grabber.ClearTarget(obj);
                }
            }
        }
    }
}
