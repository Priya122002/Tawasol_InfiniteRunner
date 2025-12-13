using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Quaternion originalRotation;
    private Coroutine shakeRoutine;

    void Awake()
    {
        originalRotation = transform.localRotation;
    }

    /// <summary>
    /// One-time Y rotation hit effect (rotate → return)
    /// </summary>
    public void HitRotateY(float angle, float duration)
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(HitRotateYRoutine(angle, duration));
    }

    IEnumerator HitRotateYRoutine(float angle, float duration)
    {
        float half = duration * 0.5f;
        float t = 0f;

        // 🔄 Rotate TO angle
        while (t < half)
        {
            float y = Mathf.Lerp(0f, angle, t / half);
            transform.localRotation = originalRotation * Quaternion.Euler(0f, y, 0f);
            t += Time.deltaTime;
            yield return null;
        }

        t = 0f;

        // 🔄 Rotate BACK to 0
        while (t < half)
        {
            float y = Mathf.Lerp(angle, 0f, t / half);
            transform.localRotation = originalRotation * Quaternion.Euler(0f, y, 0f);
            t += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = originalRotation;
        shakeRoutine = null;
    }
}
