using System.Collections;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    [Header("각 면 오브젝트")]
    public Transform front;
    public Transform back;
    public Transform left;
    public Transform right;
    public Transform top;
    public Transform bottom;

    [Header("펼침 애니메이션")]
    public float unfoldDelay = 0.7f;
    public float unfoldSpeed = 200f; // 회전 속도 (deg/sec)

    private bool isUnfolding = false;
    private bool isOpened = false;

    void Update()
    {
        // 예: 디버그용 테스트 키
        if (Input.GetKeyDown(KeyCode.U))
        {
            TryUnfold();
        }
    }

    public void TryUnfold()
    {
        if (isUnfolding || isOpened) return;
        StartCoroutine(UnfoldSequence());
    }

    IEnumerator UnfoldSequence()
    {
        isUnfolding = true;

        // 1. TOP 펼치기
        yield return StartCoroutine(RotateOverTime(top, Vector3.right, -90f));

        yield return new WaitForSeconds(unfoldDelay);

        // 2. FRONT
        yield return StartCoroutine(RotateOverTime(front, Vector3.right, 90f));
        yield return new WaitForSeconds(unfoldDelay);
              
        // 3. LEFT
        yield return StartCoroutine(RotateOverTime(left, Vector3.forward, -90f));
        yield return new WaitForSeconds(unfoldDelay);

        // 4. RIGHT
        yield return StartCoroutine(RotateOverTime(right, Vector3.forward, 90f));
        yield return new WaitForSeconds(unfoldDelay);

        // 5. BACK
        yield return StartCoroutine(RotateOverTime(back, Vector3.right, -90f));
        
        isOpened = true;
        isUnfolding = false;
    }

    IEnumerator RotateOverTime(Transform target, Vector3 axis, float angle)
    {
        Quaternion startRot = target.rotation;
        Quaternion endRot = startRot * Quaternion.AngleAxis(angle, axis);

        float time = 0f;
        while (Quaternion.Angle(target.rotation, endRot) > 0.5f)
        {
            target.rotation = Quaternion.RotateTowards(target.rotation, endRot, unfoldSpeed * Time.deltaTime);
            yield return null;
            time += Time.deltaTime;
            if (time > 2f) break; // 안전장치
        }

        target.rotation = endRot; // 정밀 정렬
    }
}
