using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfinitePlatformManager : MonoBehaviour
{
    [Header("Pool Settings")]
    public string poolTag;
    public float platformLength = 20f;
    public int initialSpawnCount = 5;

    [Header("References")]
    public Transform player;
    public Transform ghostWorldRoot;
    public bool isPlayerWorld = true;

    private float nextSpawnZ = 0f;
    private Queue<GameObject> activePlatforms = new Queue<GameObject>();

    // ⭐ REQUIRED FOR OBSTACLE DEQUEUE
    private Dictionary<GameObject, List<GameObject>> tileObstacles =
        new Dictionary<GameObject, List<GameObject>>();

    public bool IsWorldReady { get; private set; } = false;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => ObjectPooler.Instance != null && ObjectPooler.Instance.IsReady);

        for (int i = 0; i < initialSpawnCount; i++)
            SpawnPlatform();

        yield return new WaitForSeconds(0.1f);

        IsWorldReady = true;
    }

    void Update()
    {
        if (!IsWorldReady) return;
        if (player == null) return;

        GameObject firstTile = activePlatforms.Peek();

        if (player.position.z - firstTile.transform.position.z >= platformLength)
        {
            activePlatforms.Dequeue();

            float newZ = nextSpawnZ;

            // ⭐ DESPAWN OBSTACLES BELONGING TO THIS TILE
            ClearObstaclesOnTile(firstTile);

            firstTile.transform.position = new Vector3(transform.position.x, 0f, newZ);

            // ⭐ SPAWN NEW OBSTACLES IF PLAYER WORLD
            if (isPlayerWorld)
                SpawnObstaclesForTile(firstTile);

            activePlatforms.Enqueue(firstTile);
            nextSpawnZ += platformLength;
        }
    }

    void SpawnPlatform()
    {
        GameObject tile = ObjectPooler.Instance.Spawn(
            poolTag,
            new Vector3(transform.position.x, 0f, nextSpawnZ),
            Quaternion.identity
        );

        activePlatforms.Enqueue(tile);

        // ⭐ IMPORTANT: reset tile obstacle list
        tileObstacles[tile] = new List<GameObject>();

        if (isPlayerWorld)
            SpawnObstaclesForTile(tile);

        nextSpawnZ += platformLength;
    }

    // ⭐ THIS FUNCTION TRACKS AND SPAWNS OBSTACLES CORRECTLY
    private void SpawnObstaclesForTile(GameObject tile)
    {
        if (player.position.z < 10f) return; // wait until player actually runs

        ObstacleSpawner spawner = tile.GetComponent<ObstacleSpawner>();
        if (spawner == null) return;

        if (spawner.TrySpawnObstacle(player, out string tag, out Vector3 pos))
        {
            // PLAYER OBSTACLE
            GameObject obstacle = ObjectPooler.Instance.Spawn(tag, pos, Quaternion.identity);
            tileObstacles[tile].Add(obstacle);

            // GHOST OBSTACLE
            if (ghostWorldRoot != null)
            {
                Vector3 ghostPos = new Vector3(ghostWorldRoot.position.x, pos.y, pos.z);
                GameObject ghostObstacle = ObjectPooler.Instance.Spawn(tag, ghostPos, Quaternion.identity);
                tileObstacles[tile].Add(ghostObstacle);
            }
        }
    }

    // ⭐ THIS FUNCTION DEQUEUES / DISABLES OBSTACLES
    private void ClearObstaclesOnTile(GameObject tile)
    {
        if (!tileObstacles.ContainsKey(tile)) return;

        foreach (GameObject o in tileObstacles[tile])
        {
            if (o != null)
                o.SetActive(false); // return to pool
        }

        tileObstacles[tile].Clear(); // reset for next cycle
    }
}
