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
        yield return new WaitUntil(() =>
            ObjectPooler.Instance != null &&
            ObjectPooler.Instance.IsReady);

        yield return new WaitUntil(() =>
            playerManager != null && ghostManager != null &&
            playerManager.IsWorldReady && ghostManager.IsWorldReady);

        yield return SpawnCharactersAsync();
    }

    private IEnumerator SpawnCharactersAsync()
    {
        var pHandle = Addressables.InstantiateAsync(
            playerKey,
            playerSpawnPoint.position,
            playerSpawnPoint.rotation,
            playerSpawnPoint
        );

        yield return pHandle;
        playerInstance = pHandle.Result;
        playerInstance.transform.localPosition = new Vector3(0f, 0.6f, 0f);

        var gHandle = Addressables.InstantiateAsync(
            ghostKey,
            ghostSpawnPoint.position,
            ghostSpawnPoint.rotation,
            ghostSpawnPoint
        );

        yield return gHandle;
        ghostInstance = gHandle.Result;
        ghostInstance.transform.localPosition = new Vector3(0f, 0.6f, 0f);

        playerManager.player = playerInstance.transform;
        ghostManager.player = ghostInstance.transform;

        playerCamera.SetTarget(playerInstance.transform);
        ghostCamera.SetTarget(ghostInstance.transform);

        UIManager.Instance.SetPlayer(playerInstance.GetComponent<PlayerMovement>());
        ScoreManager.Instance.SetPlayer(playerInstance.transform);
        LifeSystem.Instance.RegisterPlayer(playerInstance.GetComponent<PlayerMovement>());

        var pm = playerInstance.GetComponent<PlayerMovement>();
        var gm = ghostInstance.GetComponent<PlayerMovement>();
        if (pm != null) pm.canMove = false;
        if (gm != null) gm.canMove = false;

        if (pm != null) pm.canMove = true;
        if (gm != null) gm.canMove = true;

        Debug.Log("Movement enabled for player & ghost.");
    }
}
