using UnityEngine;

public class Spawn : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject[] playerPrefabs;

    void Start()
    {
        // Lấy index lưu từ Select Scene
        int selectedPlayer = PlayerPrefs.GetInt("SelectedPlayerIndex", 0);

        // Tránh lỗi vượt mảng
        selectedPlayer = Mathf.Clamp(selectedPlayer, 0, playerPrefabs.Length - 1);

        // Spawn player
        Instantiate(playerPrefabs[selectedPlayer], spawnPoint.position, Quaternion.identity);
    }
}
