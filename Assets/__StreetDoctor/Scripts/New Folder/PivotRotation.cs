using System.Collections;
using UnityEngine;

public class PivotRotation : MonoBehaviour
{
    public Transform objectA;
    public Transform pivotB;
    public float rotationSpeed = 90f;

    private bool isRotating = false;

    public void RotateSmooth(float targetAngle)
    {
        if (!isRotating)
            StartCoroutine(RotateAroundPivotSmooth(targetAngle));
    }

    public bool IsBusy()
    {
        return isRotating;
    }

    IEnumerator RotateAroundPivotSmooth(float targetAngle)
    {
        isRotating = true;

        float rotatedAngle = 0f;
        float direction = Mathf.Sign(targetAngle);
        float absTarget = Mathf.Abs(targetAngle);

        while (rotatedAngle < absTarget)
        {
            float step = rotationSpeed * Time.deltaTime;
            if (rotatedAngle + step > absTarget)
                step = absTarget - rotatedAngle;

            objectA.RotateAround(pivotB.position, pivotB.right, step * direction);
            rotatedAngle += step;
            yield return null;
        }

        isRotating = false;
    }
}
