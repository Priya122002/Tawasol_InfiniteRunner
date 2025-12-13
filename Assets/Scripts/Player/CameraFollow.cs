using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smooth = 5f;

    private Vector3 offset;
    private bool initialized = false;

    [HideInInspector]
    public Vector3 shakeOffset;

    public void SetTarget(Transform t)
    {
        target = t;
        offset = transform.position - target.position;
        initialized = true;
    }

    void LateUpdate()
    {
        if (!initialized || target == null) return;

        Vector3 desiredPos = target.position + offset + shakeOffset;
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            smooth * Time.deltaTime
        );
    }
}
