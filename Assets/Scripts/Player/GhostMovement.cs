using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    public float targetX;
    public float targetY;
    public float targetZ;

    public float smooth = 12f;

    void Update()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Lerp(pos.x, targetX, smooth * Time.deltaTime); // LEFT/RIGHT
        pos.y = Mathf.Lerp(pos.y, targetY, smooth * Time.deltaTime); // JUMP
        pos.z = Mathf.Lerp(pos.z, targetZ, smooth * Time.deltaTime); // FORWARD

        transform.position = pos;
    }
}
