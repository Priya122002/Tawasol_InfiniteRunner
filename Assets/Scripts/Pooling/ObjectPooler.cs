using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public string addressKey;
        public int size;
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    public bool IsReady { get; private set; } = false;

    // Folder parents for organization
    private Transform floorsRoot;
    private Transform obstaclesRoot;
    private Transform coinsRoot;
    private Transform miscRoot;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Create parent groups
        floorsRoot = new GameObject("Floors_Pool").transform;
        floorsRoot.SetParent(transform);

        obstaclesRoot = new GameObject("Obstacles_Pool").transform;
        obstaclesRoot.SetParent(transform);

        coinsRoot = new GameObject("Coins_Pool").transform;
        coinsRoot.SetParent(transform);

        miscRoot = new GameObject("Misc_Pool").transform;
        miscRoot.SetParent(transform);
    }

    IEnumerator Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // Load each pool from Addressables
        foreach (Pool pool in pools)
        {
            poolDictionary.Add(pool.tag, new Queue<GameObject>());

            for (int i = 0; i < pool.size; i++)
            {
                AsyncOperationHandle<GameObject> handle =
                    Addressables.InstantiateAsync(pool.addressKey);

                yield return handle;

                GameObject obj = handle.Result;
                obj.SetActive(false);

                // Assign correct parent folder
                SetParentByTag(obj.transform, pool.tag);

                poolDictionary[pool.tag].Enqueue(obj);
            }
        }

        IsReady = true;
        Debug.Log("ObjectPooler Loaded All Addressables.");
    }

    // Assign object parent based on tag
    private void SetParentByTag(Transform obj, string tag)
    {
        if (tag.StartsWith("Floor"))
            obj.SetParent(floorsRoot);

        else if (tag.StartsWith("Obstacle"))
            obj.SetParent(obstaclesRoot);

        else if (tag.StartsWith("Coin"))
            obj.SetParent(coinsRoot);

        else
            obj.SetParent(miscRoot);
    }

    // Main spawn function
    public GameObject Spawn(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogError("Pool with tag " + tag + " does not exist!");
            return null;
        }

        GameObject obj = poolDictionary[tag].Dequeue();

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        poolDictionary[tag].Enqueue(obj); // Recycled only when manually disabled

        return obj;
    }
}
