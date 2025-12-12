using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfinitePlatformManager : MonoBehaviour
{
    [Header("Pool Settings")]
    public string poolTag;                // FloorPlayer or FloorGhost
    public float platformLength = 20f;
    public int initialSpawnCount = 5;

    [Header("Runtime References")]
    public Transform player;              // Assigned by WorldSpawner
    public Transform ghostWorldRoot;      // Offset X for ghost world
    public bool isPlayerWorld = true;     // TRUE = Player, FALSE = Ghost

    private float nextSpawnZ = 0f;
    private Queue<GameObject> activePlatforms = new Queue<GameObject>();

    public bool IsWorldReady { get; private set; } = false;

    IEnumerator Start()
    {
        // Wait until ObjectPooler has loaded all addressables
        yield return new WaitUntil(() =>
            ObjectPooler.Instance != null &&
            ObjectPooler.Instance.IsReady
        );

        // Spawn initial floor tiles
        for (int i = 0; i < initialSpawnCount; i++)
            SpawnPlatform();

        // small delay for physics settle
        yield return new WaitForSeconds(0.1f);

        IsWorldReady = true;
    }

    void Update()
    {
        if (!IsWorldReady) return;
        if (player == null) return;
        if (activePlatforms.Count == 0) return;

        GameObject firstTile = activePlatforms.Peek();

        // When player passes platform → recycle
        if (player.position.z - firstTile.transform.position.z >= platformLength)
        {
            activePlatforms.Dequeue();

            firstTile.transform.position = new Vector3(
                transform.position.x,
                0f,
                nextSpawnZ
            );

            // Spawn obstacles ONLY ON PLAYER WORLD
            if (isPlayerWorld)
                TrySpawnObstacleOnTile(firstTile);

            activePlatforms.Enqueue(firstTile);
            nextSpawnZ += platformLength;
        }
    }

    // Spawns a new platform tile + optional obstacle
    private void SpawnPlatform()
    {
        GameObject obj = ObjectPooler.Instance.Spawn(
            poolTag,
            new Vector3(transform.position.x, 0f, nextSpawnZ),
            Quaternion.identity
        );

        // Spawn obstacles for first tiles ONLY if isPlayerWorld
        if (isPlayerWorld)
            TrySpawnObstacleOnTile(obj);

        activePlatforms.Enqueue(obj);
        nextSpawnZ += platformLength;
    }

    // Main obstacle spawning logic
    private void TrySpawnObstacleOnTile(GameObject tile)
    {
        ObstacleSpawner spawner = tile.GetComponent<ObstacleSpawner>();

        if (spawner == null)
        {
            Debug.LogWarning("Tile missing ObstacleSpawner component!");
            return;
        }

        // Call ObstacleSpawner logic
        if (spawner.TrySpawnObstacle(player, out string tag, out Vector3 spawnPos))
        {
            // Spawn obstacle in Player World
            GameObject obstacle = ObjectPooler.Instance.Spawn(tag, spawnPos, Quaternion.identity);

            // Spawn mirrored obstacle in Ghost World
            if (ghostWorldRoot != null)
            {
                Vector3 ghostPos = new Vector3(
                    ghostWorldRoot.position.x,
                    spawnPos.y,
                    spawnPos.z
                );

                ObjectPooler.Instance.Spawn(tag, ghostPos, Quaternion.identity);
            }
        }
    }
}
