using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Settings")]
    public string[] obstacleTags;            // Pool tags: Obstacle1, Obstacle2, etc.

    [Header("Lane Settings")]
    public float laneOffset = 1.5f;
    public float forwardOffset = 4f;
    public float obstacleY = 0.6f;   // Height of obstacle on floor

    [Header("Rules")]
    public float minSpawnDistance = 20f;     // Player must reach this Z before obstacles begin
    public float minDistanceBetweenObstacles = 15f;

    private static float lastSpawnZ = -999f;

    public bool TrySpawnObstacle(Transform player, out string selectedTag, out Vector3 spawnPos)
    {
        selectedTag = "";
        spawnPos = Vector3.zero;

        float playerZ = player.position.z;

        // 1. Player must move at least minSpawnDistance
        if (playerZ < minSpawnDistance)
            return false;

        // 2. Respect minimum spacing between obstacles
        if (playerZ - lastSpawnZ < minDistanceBetweenObstacles)
            return false;

        // 3. Random lane selection: -1 = Left, 0 = Middle, 1 = Right
        int laneIndex = Random.Range(-1, 2);
        float laneX = laneIndex * laneOffset;

        // 4. Choose obstacle randomly
        selectedTag = obstacleTags[Random.Range(0, obstacleTags.Length)];

        // 5. Compute world space spawn position
        spawnPos = new Vector3(
            laneX,
            obstacleY,
            transform.position.z + forwardOffset
        );

        // Safety clamp (never outside floor)
        spawnPos.x = Mathf.Clamp(spawnPos.x, -laneOffset, laneOffset);

        // Record last spawn position
        lastSpawnZ = playerZ;

        return true;
    }
    public static void ResetSpawner()
    {
        lastSpawnZ = -999f;
    }


}
