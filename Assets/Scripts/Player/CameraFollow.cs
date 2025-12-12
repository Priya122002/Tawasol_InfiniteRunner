using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smooth = 5f;

    private Vector3 offset;
    private bool initialized = false;

    public void SetTarget(Transform t)
    {
        target = t;
        offset = transform.position - target.position;
        initialized = true;
    }

    void LateUpdate()
    {
        if (!initialized || target == null) return;

        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, smooth * Time.deltaTime);
    }
}
