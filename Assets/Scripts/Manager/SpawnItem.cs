using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    [Header("Danh sách prefab có thể spawn")]
    public GameObject[] itemPrefabs;

    [Header("Thời gian spawn lại")]
    public float spawnDelay = 2f;

    private List<Transform> spawnPoints = new List<Transform>();
    private GameObject currentItem;

    void Awake()
    {
        // Lấy tất cả các Point con của SpawnItem
        foreach (Transform child in transform)
        {
            spawnPoints.Add(child);
        }
    }

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (currentItem == null)
            {
                SpawnRandomItem();
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void SpawnRandomItem()
    {
        if (spawnPoints.Count == 0 || itemPrefabs.Length == 0)
            return;

        int randomPoint = Random.Range(0, spawnPoints.Count);
        int randomItem = Random.Range(0, itemPrefabs.Length);

        Transform spawnPoint = spawnPoints[randomPoint];

        currentItem = Instantiate(
            itemPrefabs[randomItem],
            spawnPoint.position,
            Quaternion.identity
        );
    }

    public void ItemCollected()
    {
        currentItem = null;
    }
}