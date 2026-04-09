using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour // Chuyển sang NetworkBehaviour
{
    public static GameManager instance;
    public GameObject retryUI;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    // Khi bất kỳ ai chết (rơi xuống vực/chạm bẫy)
    public void PlayerDied()
    {
        // Trong Multiplayer ta không dừng thời gian, chỉ hiện UI cho người đó
        ShowRetryUI();
    }

    void ShowRetryUI()
    {
        if (retryUI != null) retryUI.SetActive(true);
        // Không dùng Time.timeScale = 0 vì sẽ làm treo mạng
    }

    public void ReplayLevel()
    {
        if (IsServer)
        {
            // Server ra lệnh cho tất cả cùng load lại Scene qua Network
            NetworkManager.Singleton.SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }
        else
        {
            // Nếu là Client nhấn nút, gửi yêu cầu lên Server (Tùy chọn)
            RequestReplayServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestReplayServerRpc()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }
}