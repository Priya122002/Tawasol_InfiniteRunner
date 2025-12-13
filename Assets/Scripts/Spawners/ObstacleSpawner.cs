using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Settings")]
    public string[] obstacleTags;          

    [Header("Lane Settings")]
    public float laneOffset = 1.5f;
    public float forwardOffset = 4f;
    public float obstacleY = 0.6f;   

    [Header("Rules")]
    public float minSpawnDistance = 20f;    
    public float minDistanceBetweenObstacles = 15f;

    private static float lastSpawnZ = -999f;

    public bool TrySpawnObstacle(Transform player, out string selectedTag, out Vector3 spawnPos)
    {
        selectedTag = "";
        spawnPos = Vector3.zero;

        float playerZ = player.position.z;

        if (playerZ < minSpawnDistance)
            return false;

        if (playerZ - lastSpawnZ < minDistanceBetweenObstacles)
            return false;

        int laneIndex = Random.Range(-1, 2);
        float laneX = laneIndex * laneOffset;

        selectedTag = obstacleTags[Random.Range(0, obstacleTags.Length)];

        spawnPos = new Vector3(
            laneX,
            obstacleY,
            transform.position.z + forwardOffset
        );

        spawnPos.x = Mathf.Clamp(spawnPos.x, -laneOffset, laneOffset);

        lastSpawnZ = playerZ;

        return true;
    }
    public static void ResetSpawner()
    {
        lastSpawnZ = -999f;
    }


}
