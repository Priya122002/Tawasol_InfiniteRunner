using UnityEngine;
using System.Collections;

public class WorldSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject ghostPrefab;

    [Header("Spawn Points")]
    public Transform playerSpawnPoint;
    public Transform ghostSpawnPoint;

    public CameraFollow playerCamera;
    public CameraFollow ghostCamera;

    private GameObject playerInstance;
    private GameObject ghostInstance;

    public InfinitePlatformManager playerManager;
    public InfinitePlatformManager ghostManager;

    void Start()
    {
        StartCoroutine(SpawnAfterPlatforms());
    }

    IEnumerator SpawnAfterPlatforms()
    {
        // Wait until floor tiles exist
        while (ObjectPooler.Instance.transform.childCount == 0)
            yield return null;

        SpawnCharacters();
    }
    void SpawnCharacters()
    {
        // --- Spawn Player ---
        playerInstance = Instantiate(
            playerPrefab,
            playerSpawnPoint.position,
            playerSpawnPoint.rotation,
            playerSpawnPoint     // parent
        );

        // Force X = 0 inside parent local space
        playerInstance.transform.localPosition = new Vector3(
            0f,
            0.6f,     // height offset
            0f
        );

        // --- Spawn Ghost ---
        ghostInstance = Instantiate(
            ghostPrefab,
            ghostSpawnPoint.position,
            ghostSpawnPoint.rotation,
            ghostSpawnPoint     // parent
        );

        // Force X = 0 inside parent local space
        ghostInstance.transform.localPosition = new Vector3(
            0f,
            0.6f,
            0f
        );

        // Assign to managers
        playerManager.player = playerInstance.transform;
        ghostManager.player = ghostInstance.transform;

        // Assign cameras
        playerCamera.SetTarget(playerInstance.transform);
        ghostCamera.SetTarget(ghostInstance.transform);
    }

}
