using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int orbScore = 0;
    public float distanceScore = 0;

    private Transform player;
    private float startZ;
    public bool countingEnabled = true;

    void Awake()
    {
        Instance = this;
    }

    public void SetPlayer(Transform p)
    {
        player = p;
        startZ = p.position.z;   // IMPORTANT — starting point
    }

    void Update()
    {
        if (player != null && countingEnabled)
        {
            distanceScore = (player.position.z - startZ) * 0.05f;

            if (distanceScore < 0) distanceScore = 0;   // safety
        }
    }

    public int DistanceInt => Mathf.FloorToInt(distanceScore);

    public void AddOrb(int amount)
    {
        orbScore += amount;
    }

    public int TotalScore => DistanceInt + orbScore;

    public void StopScoring()
    {
        countingEnabled = false;
    }
}
