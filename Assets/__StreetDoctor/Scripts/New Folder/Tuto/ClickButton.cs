using System.Collections;
using UnityEngine;

public class ClickButton : MonoBehaviour
{
    private Vector3 originalPosition;
    public float pressDepth = 0.003f;
    public float pressDuration = 0.1f;

    private bool isPressed = false;

    [Header ("BtnOpen용 설정 ")]
    public PivotRotation lidPivot; // 뚜껑 회전 스크립트 연결
    private bool isLidOpen = false; // 뚜껑 상태 기억

    [Header ("머티리얼 컨트롤용 설정")]
    private Renderer myRenderer;
    private Material[] defaultMats;

    public GameObject BtnAgeAOBJ;
    public GameObject BtnAgeKOBJ;
    public Material secondMaterial;

    [Header("자동문 관련")]
    public Transform doorLeft;
    public Transform doorRight;
    public float doorOpenDistance = 2.5f;
    public float doorOpenSpeed = 2.0f;
    private bool isDoorOpen = false;
    private bool doorLock = false;
    private float doorOpenDuration = 30f; // 30초

    void Start()
    {
        originalPosition = transform.localPosition;

        myRenderer = GetComponent<Renderer>();
        //if (myRenderer != null)
        //{
        //    defaultMats = myRenderer.materials;

        //    // 시작 시 element 1을 완전히 제거 (핑크 방지)
        //    if (defaultMats.Length >= 2)
        //    {
        //        Material[] mats = new Material[1];
        //        mats[0] = defaultMats[0]; // 메인 머티리얼만 유지
        //        myRenderer.materials = mats;
        //    }
        //}
        GameObject left = GameObject.Find("DoorLeft");
        GameObject right = GameObject.Find("DoorRight");

        if (left != null && right != null)
        {
            Debug.Log("문 자동 연결 성공!");
            doorLeft = left.transform;
            doorRight = right.transform;
        }
        else
        {
            Debug.LogWarning("자동문 오브젝트를 찾지 못했습니다. 'DoorLeft', 'DoorRight' 이름 확인하세요.");
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand") && !isPressed)
        {
            StartCoroutine(PressAnimation());

            switch (gameObject.name)
            {
                case "BtnOpen":
                    if (lidPivot != null && !lidPivot.IsBusy())
                    {
                        if (!isLidOpen)
                        {
                            lidPivot.RotateSmooth(90f); // 열기
                            isLidOpen = true;
                        }
                        else
                        {
                            lidPivot.RotateSmooth(-90f); // 닫기
                            isLidOpen = false;
                        }
                    }
                    break;

                case "BtnR":
                    Debug.Log("AED를 시작합니다.");
                    break;

                case "BtnShock":
                    Debug.Log("찌릿 찌릿");
                    break;

                case "BtnAgeA":
                    SetMaterialState(BtnAgeAOBJ, true);  // A 활성화
                    SetMaterialState(BtnAgeKOBJ, false); // K 비활성화
                    break;

                case "BtnAgeK":
                    SetMaterialState(BtnAgeAOBJ, false); // A 비활성화
                    SetMaterialState(BtnAgeKOBJ, true);  // K 활성화
                    break;

                default:
                    Debug.Log($"{gameObject.name} 버튼이 눌림!");
                    break;

                case "BtnDoor":                   
                    if (!isDoorOpen && !doorLock)
                    {
                        Debug.Log("문 열기 시도");
                        StartCoroutine(OpenAutoDoor());
                        isDoorOpen = true;
                        doorLock = true;
                        StartCoroutine(AutoCloseDoorAfterDelay());
                    }
                    else
                    {
                            //StartCoroutine(CloseAutoDoor());
                            //isDoorOpen = false;                       
                    }
                    break;
            }
        }
    }

    void SetMaterialState(GameObject targetObj, bool enableSecondMat)
    {
        if (targetObj == null) return;

        Renderer rend = targetObj.GetComponent<Renderer>();
        if (rend == null) return;

        Material[] newMats;

        if (enableSecondMat && secondMaterial != null)
        {
            newMats = new Material[2];
            newMats[0] = rend.sharedMaterials[0];  // 원래 메인
            newMats[1] = secondMaterial;           // Element 1 활성화
        }
        else
        {
            newMats = new Material[1];
            newMats[0] = rend.sharedMaterials[0];  // Element 1 제거 (핑크 방지)
        }

        rend.materials = newMats;
    }

    IEnumerator PressAnimation()
    {
        isPressed = true;

        Vector3 downPos = originalPosition + new Vector3(0, -pressDepth, 0);
        float t = 0;
        while (t < pressDuration)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(originalPosition, downPos, t / pressDuration);
            yield return null;
        }

        t = 0;
        while (t < pressDuration)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(downPos, originalPosition, t / pressDuration);
            yield return null;
        }
        transform.localPosition = originalPosition;
        isPressed = false;
    }
    IEnumerator OpenAutoDoor()
    {        
        Vector3 leftTarget = doorLeft.position + doorLeft.right * doorOpenDistance;
        Vector3 rightTarget = doorRight.position + doorRight.right * -doorOpenDistance;
        float t = 0;
        Vector3 leftStart = doorLeft.position;
        Vector3 rightStart = doorRight.position;       

        while (t < 1f)
        {
            t += Time.deltaTime * doorOpenSpeed;
            doorLeft.position = Vector3.Lerp(leftStart, leftTarget, t);
            doorRight.position = Vector3.Lerp(rightStart, rightTarget, t);
            yield return null;
        }
    }
    IEnumerator AutoCloseDoorAfterDelay()
    {
        yield return new WaitForSeconds(doorOpenDuration);
        Debug.Log("자동으로 문 닫기 시도");
        StartCoroutine(CloseAutoDoor());
        isDoorOpen = false;
        doorLock = false;
    }
    IEnumerator CloseAutoDoor()
    {
        Vector3 leftStart = doorLeft.position;
        Vector3 rightStart = doorRight.position;
        Vector3 leftTarget = doorLeft.position + doorLeft.right * -doorOpenDistance;
        Vector3 rightTarget = doorRight.position + doorRight.right * doorOpenDistance;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * doorOpenSpeed;
            doorLeft.position = Vector3.Lerp(leftStart, leftTarget, t);
            doorRight.position = Vector3.Lerp(rightStart, rightTarget, t);
            yield return null;
        }
    }
}
