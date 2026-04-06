using UnityEngine;
using Unity.Netcode;

public class Spawn : NetworkBehaviour
{
    public Transform[] spawnPoints; // nhiều vị trí spawn
    public GameObject[] playerPrefabs;
    public HealthBarController healthBarController;

    public override void OnNetworkSpawn()
    {
        
        // chỉ server spawn
        Debug.Log("== Check object spawned");

        if (!IsHost) return;

        Debug.Log("== Check spawn player for client");
        SpawnAllPlayers();
    }

    void SpawnAllPlayers()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Debug.Log($"== Spawn for player: {clientId}");
            SpawnPlayer(clientId);
        }
    }

    void SpawnPlayer(ulong clientId)
    {
        // chọn prefab random
        int randomIndex = Random.Range(0, playerPrefabs.Length);

        // chọn vị trí spawn random
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject player = Instantiate(playerPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);

        NetworkObject netObj = player.GetComponent<NetworkObject>();

        // spawn player cho client đó
        netObj.SpawnAsPlayerObject(clientId, true);
    }
}