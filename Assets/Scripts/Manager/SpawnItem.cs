using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnItem : NetworkBehaviour
{
    public GameObject[] itemPrefabs;
    public float spawnDelay = 3f;
    private List<Transform> spawnPoints = new List<Transform>();
    private GameObject currentSpawnedItem;

    void Awake()
    {
        foreach (Transform child in transform) spawnPoints.Add(child);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer) StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            // Nếu quả cũ đã bị Despawn (null), thì mới spawn quả mới
            if (currentSpawnedItem == null)
            {
                SpawnRandomItem();
            }
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void SpawnRandomItem()
    {
        if (itemPrefabs.Length == 0) return;

        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Count)];
        GameObject prefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];

        currentSpawnedItem = Instantiate(prefab, sp.position, Quaternion.identity);
        // Quan trọng nhất: Gọi Spawn() để đối tượng xuất hiện trên tất cả máy Client
        currentSpawnedItem.GetComponent<NetworkObject>().Spawn();
    }
}