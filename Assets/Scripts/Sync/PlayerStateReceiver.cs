using UnityEngine;

public class PlayerStateReceiver : MonoBehaviour
{
    public float networkDelay = 0.1f;

    private int readIndex = 0;
    private PlayerState latest;

    private GhostMovement ghost;

    void Start()
    {
        ghost = GetComponent<GhostMovement>();
    }

    void Update()
    {
        int nextIndex = PlayerStateSender.writeIndex;

        if (readIndex == nextIndex) return; // no new state

        latest = PlayerStateSender.buffer[readIndex];

        ghost.targetX = latest.x;   // THIS WAS MISSING
        ghost.targetY = latest.y;
        ghost.targetZ = latest.z;

        readIndex = (readIndex + 1) % PlayerStateSender.buffer.Length;
    }
}
