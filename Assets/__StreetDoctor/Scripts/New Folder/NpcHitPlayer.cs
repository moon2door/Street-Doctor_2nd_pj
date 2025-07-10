using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcHitPlayer : MonoBehaviour
{
    public RandomPatrol patrol;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ✅ 매니저에게 먼저 허락 요청
            if (!NpcManager.Instance.TryTriggerNPC(patrol))
                return;

            // ✅ 직속 부모의 Collider 비활성화
            var colliders = transform.parent?.GetComponents<Collider>();
            if (colliders != null)
            {
                foreach (var col in colliders)
                    col.enabled = false;
            }

            patrol.StopAndRotate90();
        }

    }
}
