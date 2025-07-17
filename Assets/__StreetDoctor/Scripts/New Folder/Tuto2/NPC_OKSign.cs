using UnityEngine;

public class NPC_OKSign : MonoBehaviour
{
    public ForwardRayLine forward;
    public Animator myAnim;

    public bool isBlinking = false;

    private void Start()
    {
        myAnim = GetComponent<Animator>();
        forward = GameObject.Find("XRHand_IndexTipR").GetComponent<ForwardRayLine>();
    }

    void Update()
    {
        if (forward == null) return;

        isBlinking = forward.isBlink;
    }

    public void PlayPhoneAnim()
    {
        if (myAnim != null)
            myAnim.SetTrigger("Phone_T");
        myAnim.applyRootMotion = true;
    }
}
