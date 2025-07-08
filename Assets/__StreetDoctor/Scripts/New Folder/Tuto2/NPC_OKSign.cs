using UnityEngine;

public class NPC_OKSign : MonoBehaviour
{
    public ForwardRayLine forward;

    public bool isBlinking = false;

    void Update()
    {
        if (forward != null)
        {
            if (forward.isBlink == true)
            {
                isBlinking = true;
            }
            else
            {
                isBlinking = false;
            }
        }
        else
        {
            return;
        }
    }
}
