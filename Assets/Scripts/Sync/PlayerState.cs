[System.Serializable]
public struct PlayerState
{
    public float z;      // forward progress
    public float y;      // vertical jump height
    public bool jumped;  // jump event
    public float time;

    public PlayerState(float _z, float _y, bool j, float t)
    {
        z = _z;
        y = _y;
        jumped = j;
        time = t;
    }
}
