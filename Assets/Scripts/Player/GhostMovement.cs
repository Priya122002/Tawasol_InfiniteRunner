using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    [HideInInspector] public float targetZ;
    [HideInInspector] public float targetY;

    public float smooth = 12f;

    void Update()
    {
        Vector3 pos = transform.position;

        // Follow Z smoothly
        pos.z = Mathf.Lerp(pos.z, targetZ, smooth * Time.deltaTime);

        // Follow Y exactly like the player (no gravity)
        pos.y = Mathf.Lerp(pos.y, targetY, smooth * Time.deltaTime);

        transform.position = pos;
    }

}
