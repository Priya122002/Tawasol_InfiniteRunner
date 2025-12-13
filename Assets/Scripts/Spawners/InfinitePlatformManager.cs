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

    private Dictionary<GameObject, List<GameObject>> tileObstacles =
        new Dictionary<GameObject, List<GameObject>>();

    public bool IsWorldReady { get; private set; } = false;
    void OnEnable()
    {
        GetComponent<ObstacleHit>()?.ResetHit();
        GetComponent<ObstacleDissolve>()?.ResetDissolve();
    }

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

            ClearObstaclesOnTile(firstTile);

            firstTile.transform.position = new Vector3(transform.position.x, 0f, newZ);

            if (isPlayerWorld)
                SpawnObstaclesForTile(firstTile);
            SpawnOrb(firstTile);

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

        tileObstacles[tile] = new List<GameObject>();

        if (isPlayerWorld)
            SpawnObstaclesForTile(tile);

        nextSpawnZ += platformLength;
    }

    private void SpawnObstaclesForTile(GameObject tile)
    {
        if (player.position.z < 10f) return;

        ObstacleSpawner spawner = tile.GetComponent<ObstacleSpawner>();
        if (spawner == null) return;

        if (spawner.TrySpawnObstacle(player, out string tag, out Vector3 pos))
        {
            GameObject obstacle = ObjectPooler.Instance.Spawn(tag, pos, Quaternion.identity);
            tileObstacles[tile].Add(obstacle);

            if (ghostWorldRoot != null)
            {
                Vector3 ghostPos = new Vector3(ghostWorldRoot.position.x, pos.y, pos.z);
                GameObject ghostObstacle = ObjectPooler.Instance.Spawn(tag, ghostPos, Quaternion.identity);
                tileObstacles[tile].Add(ghostObstacle);
            }
        }
    }


    private void ClearObstaclesOnTile(GameObject tile)
    {
        if (!tileObstacles.ContainsKey(tile)) return;

        foreach (GameObject o in tileObstacles[tile])
        {
            if (o != null)
                o.SetActive(false); 
        }

        tileObstacles[tile].Clear();
    }
    private void SpawnOrb(GameObject tile)
    {
        if (player.position.z < 10f)
            return;

        if (Random.value > 0.30f)
            return;

        int lane = Random.Range(-1, 2);
        float laneOffset = 1.5f;

        float spawnZ = tile.transform.position.z +
                       Random.Range(platformLength * 0.5f, platformLength - 2f);

        Vector3 pos = new Vector3(
            lane * laneOffset,
            1.0f,
            spawnZ
        );

        GameObject orb = ObjectPooler.Instance.Spawn("Orb", pos, Quaternion.identity);
        tileObstacles[tile].Add(orb);
    }

    public void ResetWorld()
    {
        nextSpawnZ = 0f;
        activePlatforms.Clear();
        tileObstacles.Clear();
        IsWorldReady = false;
    }


}