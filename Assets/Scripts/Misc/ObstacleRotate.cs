using UnityEngine;
using System.Collections;

public class ObstacleRotate : MonoBehaviour
{
    public float zRotationSpeed = 180f;
    public float xRotationSpeed = 90f;

    private bool zRotationDone;
    private Coroutine rotateRoutine;

    void OnEnable()
    {
        zRotationDone = false;

        // Reset rotation when reused from pool
        transform.localEulerAngles = Vector3.zero;

        rotateRoutine = StartCoroutine(RotateZThenX());
    }

    void OnDisable()
    {
        if (rotateRoutine != null)
            StopCoroutine(rotateRoutine);
    }

    IEnumerator RotateZThenX()
    {
        // Step 1: Rotate Z → 90°
        while (!zRotationDone)
        {
            Vector3 euler = transform.localEulerAngles;
            euler.z = Mathf.MoveTowardsAngle(euler.z, 90f, zRotationSpeed * Time.deltaTime);
            transform.localEulerAngles = euler;

            if (Mathf.Abs(Mathf.DeltaAngle(euler.z, 90f)) < 0.5f)
                zRotationDone = true;

            yield return null;
        }

        // Step 2: Continuous X-axis rotation
        while (true)
        {
            transform.Rotate(xRotationSpeed * Time.deltaTime, 0f, 0f, Space.Self);
            yield return null;
        }
    }
}
