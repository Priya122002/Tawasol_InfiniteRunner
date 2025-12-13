using UnityEngine;

public class PlayerStateSender : MonoBehaviour
{
    public static PlayerState[] buffer = new PlayerState[256];
    public static int writeIndex = 0;

    public float sendInterval = 0.02f;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= sendInterval)
        {
            timer = 0f;

            buffer[writeIndex] = new PlayerState(
                transform.position.x,   // lane
                transform.position.y,   // jump
                transform.position.z,   // forward
                Time.time
            );

            writeIndex = (writeIndex + 1) % buffer.Length;
        }
    }
    public static void ResetBuffer()
    {
        writeIndex = 0;
        for (int i = 0; i < buffer.Length; i++)
            buffer[i] = default;
    }

}
