using System.Collections;
using UnityEngine;

public class CPRMain : MonoBehaviour
{
    private bool isInStartZone = false;
    private bool isInEndZone = false;
    private bool wasInEndZone = false;

    public GameObject startObj;
    public GameObject endObj;
    public GameObject reObj;
    public float cprCount = 0;

    void Start()
    {
        startObj = GameObject.FindWithTag("CPR_Start");
        endObj = GameObject.FindWithTag("CPR_End");
        reObj = GameObject.FindWithTag("CPR_Re");

        StartCoroutine(AO_Active());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CPR_Start"))
        {
            isInStartZone = true;
            endObj.SetActive(true);
            startObj.SetActive(false);

            Debug.Log("CPR ������ ���� - A ������Ʈ Ȱ��ȭ");
        }

        else if (other.CompareTag("CPR_End"))
        {
            isInEndZone = true;

            Debug.Log("CPR �� �� ����");
        }

        else if (other.CompareTag("CPR_Re"))
        {
            StartCoroutine(AO_Active());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CPR_End"))
        {
            isInEndZone = false;

            if (wasInEndZone && isInStartZone)
            {
                cprCount++;
                Debug.Log($"CPR ī��Ʈ ����: {cprCount}");

                isInStartZone = false;

                endObj.SetActive(false);
                reObj.SetActive(true);
            }
        }
    }

    void Update()
    {
        // ������ �����ӿ��� EndZone�� �־����� ����
        wasInEndZone = isInEndZone;
    }

    IEnumerator AO_Active()
    {
        yield return new WaitForSeconds(0.1f);

        endObj.SetActive(false);
        reObj.SetActive(false);
        startObj.SetActive(true);
    }
}