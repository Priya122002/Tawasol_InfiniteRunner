[System.Serializable]
public struct PlayerState
{
    public float x;
    public float y;
    public float z;
    public float time;

    public PlayerState(float _x, float _y, float _z, float _time)
    {
        x = _x;
        y = _y;
        z = _z;
        time = _time;
    }
}
