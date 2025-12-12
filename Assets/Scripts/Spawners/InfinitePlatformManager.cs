using UnityEngine;
using System.Collections.Generic;

public class InfinitePlatformManager : MonoBehaviour
{
    [Header("Pool Settings")]
    public string poolTag;            // Example: "Platform_Player"
    public float platformLength = 20f;
    public int initialSpawnCount = 5;

    [Header("Runtime References")]
    public Transform player;          // SET AT RUNTIME by WorldSpawner

    private float nextSpawnZ = 0f;
    private Queue<GameObject> activePlatforms = new Queue<GameObject>();

    void Start()
    {
        // 🔥 Spawn initial tiles
        for (int i = 0; i < initialSpawnCount; i++)
            SpawnPlatform();
    }

    void Update()
    {
        if (player == null) return;   // ❌ If player not assigned, pooling will never happen

        GameObject firstTile = activePlatforms.Peek();

        // 🔥 Correct recycle condition
        if (player.position.z - firstTile.transform.position.z >= platformLength)
        {
            activePlatforms.Dequeue();

            firstTile.transform.position = new Vector3(
                transform.position.x,
                0f,
                nextSpawnZ
            );

            activePlatforms.Enqueue(firstTile);
            nextSpawnZ += platformLength;
        }
    }

    void SpawnPlatform()
    {
        GameObject obj = ObjectPooler.Instance.Spawn(
            poolTag,
            new Vector3(transform.position.x, 0f, nextSpawnZ),
            Quaternion.identity
        );

        activePlatforms.Enqueue(obj);
        nextSpawnZ += platformLength;
    }
}
