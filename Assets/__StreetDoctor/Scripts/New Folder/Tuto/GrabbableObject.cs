using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    [HideInInspector] public bool isGrabbed = false;

    [Header("릴리즈 시 붙을 부모")]
    public Transform returnParent;
    private Vector3 savedPosition;

    private bool hasTriggered = false;    
    //private Quaternion savedRotation;
    void Start()
    {
        savedPosition = transform.position;
        //savedRotation = transform.rotation;
    }
    public void Grab(Transform handTransform)
    {
        isGrabbed = true;
        if (returnParent == null)
        {
            returnParent = transform.parent;
            savedPosition = transform.position; // 혹시 모를 대비
        }

        transform.SetParent(handTransform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // 잡을 땐 중력 비활성화 (움직임만 따라가게)
        GetComponent<Rigidbody>().isKinematic = true;
    }
    
    public void Release()
    {
        isGrabbed = false;
        transform.SetParent(null);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; // 이제 중력 받기 시작
        }   
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || CubeManager.Instance == null || CubeManager.Instance.IsCompleted) return;

        if (other.CompareTag("BasketZone"))
        {
            hasTriggered = true;
            Debug.Log("[GrabbableObject] 바구니 트리거 감지 → 카운트 후 리셋");
            CubeManager.Instance.OnCubeEnteredBasket(this);
        }
        else if (other.CompareTag("GroundZone"))
        {
            Debug.Log("[GrabbableObject] 바닥 트리거 감지 → 원위치 복귀");
            ReturnToOriginalPosition();
        }
    }    
    //void OnCollisionEnter(Collision collision)
    //{
    //    if (!isGrabbed) return;

    //    if (collision.collider.CompareTag("Ground"))
    //    {
    //        ReturnToOriginalPosition();
    //    }
    //    else if (collision.collider.CompareTag("Basket") && !isAlreadyInBasket)
    //    {
    //        isAlreadyInBasket = true;
    //        CubeManager.Instance.OnCubeEnteredBasket(this);
    //    }
    //}

    void ReturnToOriginalPosition()
    {
        transform.SetParent(null);
        transform.position = savedPosition;
        transform.rotation = Quaternion.identity;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // 다시 공중에 고정
        }
        hasTriggered = false;       
    }
    public void ResetState()
    {
        savedPosition = transform.position;        
        hasTriggered = false;
    }
}
//        //  현재 위치 근처에 ZoneSnapper가 있는지 검사
//        Collider[] hits = Physics.OverlapSphere(transform.position, 0.1f);
//        foreach (var col in hits)
//        {
//            ZoneSnapper snapper = col.GetComponent<ZoneSnapper>();
//            if (snapper != null)
//            {
//                ApplyRandomRotation(); //    Snap 전 회전 적용
//                snapper.SnapObject(gameObject); // 스냅 위치로 이동                
//                return; // 원래 자리로 돌아가지 않고 여기서 끝냄
//            }
//        }       
//        //  스냅 실패 → 원래 위치로 돌아감
//        if (returnParent != null)
//        {            
//            transform.SetParent(null); // 자식 끊고
//            transform.position = savedPosition;
//            ApplyRandomRotation(); //  복귀 전에도 회전 적용
//        }
//        else
//        {
//            transform.SetParent(null);
//        }       
//    }
//    void ApplyRandomRotation()
//    {
//        float[] angles = new float[] { 0f, 90f, 180f, 270f };
//        float x = angles[Random.Range(0, 4)];
//        float y = angles[Random.Range(0, 4)];
//        transform.rotation = Quaternion.Euler(x, y, 0f);
//    }
//}
