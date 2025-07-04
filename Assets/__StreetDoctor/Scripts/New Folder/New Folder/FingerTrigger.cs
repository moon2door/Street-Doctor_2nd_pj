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

    void Start()
    {
        //phoneText = GameObject.Find("Phone_T").GetComponent<Text>();
        cprTS = GameObject.Find("Start Zone").GetComponent<CPRTraningStart>();
    }

    public void Setup(HandGrabber grabber, bool isLeft)
    {
        this.grabber = grabber;
        this.isLeftHand = isLeft;
    }

    void OnTriggerEnter(Collider other)
    {
        // Grab 대상 처리
        if (other.CompareTag("Grabbable"))
        {
            GrabbableObject obj = other.GetComponent<GrabbableObject>();
            if (obj != null)
                grabber.SetTarget(obj, isLeftHand);
        }

        if (isLeftHand) return;
        if (phoneText == null) return;

        // ⏱️ 0.5초 쿨타임 체크
        if (Time.time - lastInputTime < 0.5f) return;
        lastInputTime = Time.time;

        // 번호 키패드 처리
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
