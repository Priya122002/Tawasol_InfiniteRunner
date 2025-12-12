using UnityEngine;

public class PlayerStateSender : MonoBehaviour
{
    public static PlayerState[] buffer = new PlayerState[128];
    public static int writeIndex = 0;

    public float sendInterval = 0.02f;
    private float timer;

    private PlayerMovement movement;

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= sendInterval)
        {
            timer = 0f;

            buffer[writeIndex] = new PlayerState(
                transform.position.z,     // z forward
                transform.position.y,     // y height from jump
                movement.IsJumping,       // jump event
                Time.time                 // timestamp
            );

            writeIndex = (writeIndex + 1) % buffer.Length;
        }
    }
}
