using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class SpawnManager : NetworkBehaviour
{
    public Transform[] spawnPoints;
    public GameObject[] playerPrefabs;

    public override void OnNetworkSpawn()
    {
        // 1. DÀNH CHO SERVER: Bổ sung nhân vật cho người mới vào (Ví dụ ở màn 1)
        if (IsServer)
        {
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (client.PlayerObject == null) SpawnPlayer(client.ClientId);
            }

            NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
        }

        // 2. DÀNH CHO TẤT CẢ MỌI NGƯỜI: Tự đánh thức nhân vật của mình khi qua màn
        StartCoroutine(SelfWakeUpLocal());
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayer;
    }

    void SpawnPlayer(ulong clientId)
    {
        // Chỉ đẻ ra nhân vật mới nếu nó chưa tồn tại
        if (NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject != null) return;

        int index = (int)clientId % playerPrefabs.Length;
        Transform spawnPoint = spawnPoints[(int)clientId % spawnPoints.Length];

        GameObject playerInstance = Instantiate(playerPrefabs[index], spawnPoint.position, Quaternion.identity);
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    }

    // --- HÀM TỰ ĐÁNH THỨC CỰC KỲ AN TOÀN ---
    IEnumerator SelfWakeUpLocal()
    {
        // Chờ nửa giây để màn hình load xong xuôi 100%
        yield return new WaitForSeconds(0.5f);

        // 1. CHỈ BẬT SCRIPT VÀ VẬT LÝ, KHÔNG ĐƯỢC CHẠM VÀO TỌA ĐỘ CỦA NGƯỜI KHÁC
        PlayerNetwork[] allPlayers = FindObjectsOfType<PlayerNetwork>();
        foreach (var p in allPlayers)
        {
            p.enabled = true;
            if (p.rb != null)
            {
                p.rb.simulated = true;
                p.rb.velocity = Vector2.zero;
            }
        }

        // 2. CHỈ CÓ CHỦ NHÂN MỚI TỰ DỊCH CHUYỂN NHÂN VẬT CỦA CHÍNH MÌNH
        if (NetworkManager.Singleton.LocalClient != null && NetworkManager.Singleton.LocalClient.PlayerObject != null)
        {
            // Lấy ID và nhân vật của chính máy mình
            ulong myId = NetworkManager.Singleton.LocalClientId;
            NetworkObject myNetObj = NetworkManager.Singleton.LocalClient.PlayerObject;
            PlayerNetwork myPlayer = myNetObj.GetComponent<PlayerNetwork>();

            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                // Tìm đúng điểm Spawn dành cho mình
                Transform mySpawn = spawnPoints[(int)myId % spawnPoints.Length];

                // Ép Z = 0 để chống tàng hình
                Vector3 myPos = mySpawn.position;
                myPos.z = 0f;

                // TỰ DỊCH CHUYỂN BẢN THÂN
                myPlayer.transform.position = myPos;

                // Cập nhật Checkpoint
                PlayerHealth h = myNetObj.GetComponent<PlayerHealth>();
                if (h != null) h.SetNewSpawnPoint(myPos);
            }
        }
    }
}