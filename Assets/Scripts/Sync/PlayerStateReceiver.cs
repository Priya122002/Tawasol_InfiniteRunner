using UnityEngine;

public class PlayerStateReceiver : MonoBehaviour
{
    public float networkDelay = 0.1f;
    public float smooth = 12f;

    int readIndex = 0;
    PlayerState target;

    GhostMovement ghost;

    void Start()
    {
        ghost = GetComponent<GhostMovement>();
    }

    void Update()
    {
        while (true)
        {
            PlayerState next = PlayerStateSender.buffer[readIndex];

            if (next.time == 0)
                break;

            if (next.time >= Time.time - networkDelay)
            {
                target = next;
                break;
            }

            readIndex = (readIndex + 1) % PlayerStateSender.buffer.Length;
        }

        // Send Y + Z to ghost
        ghost.targetZ = target.z;
        ghost.targetY = target.y;
    }

}
