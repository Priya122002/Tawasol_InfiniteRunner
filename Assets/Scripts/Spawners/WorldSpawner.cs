using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;

public class WorldSpawner : MonoBehaviour
{
    [Header("Address Keys")]
    public string playerKey = "Player";
    public string ghostKey = "Ghost";

    [Header("Spawn Points")]
    public Transform playerSpawnPoint;
    public Transform ghostSpawnPoint;

    [Header("Cameras")]
    public CameraFollow playerCamera;
    public CameraFollow ghostCamera;

    [Header("Platform Managers (assign in Inspector)")]
    public InfinitePlatformManager playerManager;
    public InfinitePlatformManager ghostManager;

    private GameObject playerInstance;
    private GameObject ghostInstance;

    private IEnumerator Start()
    {
        // Wait until object pooler has at least started (optional)
        yield return new WaitUntil(() =>
            ObjectPooler.Instance != null &&
            ObjectPooler.Instance.IsReady);

        // Wait until both platform managers finish spawning terrain
        yield return new WaitUntil(() =>
            playerManager != null && ghostManager != null &&
            playerManager.IsWorldReady && ghostManager.IsWorldReady);

        // At this point entire terrain for both worlds is ready → spawn characters
        yield return SpawnCharactersAsync();
    }

    private IEnumerator SpawnCharactersAsync()
    {
        // --- PLAYER ---
        var pHandle = Addressables.InstantiateAsync(
            playerKey,
            playerSpawnPoint.position,
            playerSpawnPoint.rotation,
            playerSpawnPoint
        );

        yield return pHandle;
        playerInstance = pHandle.Result;
        playerInstance.transform.localPosition = new Vector3(0f, 0.6f, 0f);

        // --- GHOST ---
        var gHandle = Addressables.InstantiateAsync(
            ghostKey,
            ghostSpawnPoint.position,
            ghostSpawnPoint.rotation,
            ghostSpawnPoint
        );

        yield return gHandle;
        ghostInstance = gHandle.Result;
        ghostInstance.transform.localPosition = new Vector3(0f, 0.6f, 0f);

        // Assign references to platform managers so recycling can use player z
        playerManager.player = playerInstance.transform;
        ghostManager.player = ghostInstance.transform;

        // Set camera targets
        playerCamera.SetTarget(playerInstance.transform);
        ghostCamera.SetTarget(ghostInstance.transform);

        // Register input manager if you have one
        UIManager.Instance.SetPlayer(playerInstance.GetComponent<PlayerMovement>());
        ScoreManager.Instance.SetPlayer(playerInstance.transform);

        // Ensure player/ghost movement is disabled immediately
        var pm = playerInstance.GetComponent<PlayerMovement>();
        var gm = ghostInstance.GetComponent<PlayerMovement>();
        if (pm != null) pm.canMove = false;
        if (gm != null) gm.canMove = false;

        // enable movement
        if (pm != null) pm.canMove = true;
        if (gm != null) gm.canMove = true;

        Debug.Log("Movement enabled for player & ghost.");
    }
}
