using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections;

public class CountdownTimer : NetworkBehaviour
{
    [Header("UI")] [SerializeField] private TMP_Text timeText;

    [Header("Time Settings")] [SerializeField]
    private float startTime = 60f;

    // Biến đồng bộ thời gian từ Server xuống tất cả Client
    private NetworkVariable<float> currentTime = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private bool hasEnded = false;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentTime.Value = startTime;
            hasEnded = false;
        }
    }

    void Update()
    {
        // CHỈ SERVER mới được phép đếm thời gian
        if (IsServer && !hasEnded)
        {
            currentTime.Value -= Time.deltaTime;

            if (currentTime.Value <= 0f)
            {
                currentTime.Value = 0f;
                hasEnded = true;

                // Server thực hiện luồng kết thúc game
                StartCoroutine(EndGameFlow());
            }
        }

        // Mọi máy đều tự cập nhật UI của riêng mình
        UpdateUI();
    }

    void UpdateUI()
    {
        if (timeText == null) return;

        // Dùng CeilToInt để hiển thị thời gian thân thiện
        int seconds = Mathf.CeilToInt(currentTime.Value);
        // Tránh việc giây hiển thị số âm trên màn hình Client
        timeText.text = Mathf.Max(0, seconds).ToString();
    }

    IEnumerator EndGameFlow()
    {
        Debug.Log("--- HẾT GIỜ! ĐANG XỬ LÝ KẾT THÚC ---");

        // 1. Lưu điểm cục bộ trên máy Host (Server)
        int finalScore = 0;
        if (Score.instance != null)
        {
            finalScore = Score.instance.GetScore();
        }

        string sceneName = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetInt(sceneName + "_Score", finalScore);
        PlayerPrefs.SetString("LastLevel", sceneName);
        PlayerPrefs.Save();

        Debug.Log("Saved Score: " + finalScore);

        // 2. NGAY LẬP TỨC ĐÓNG BĂNG MỌI NGƯỜI CHƠI (Chống lỗi va chạm lúc đang load scene)
        FreezeAllPlayersClientRpc();

        // 3. Chờ 0.5s để chắc chắn điểm và gói tin cuối cùng được cập nhật
        yield return new WaitForSeconds(0.5f);

        // 4. SERVER ra lệnh cho toàn bộ Network chuyển sang Scene Leaderboard
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
        {
            var status = NetworkManager.Singleton.SceneManager.LoadScene("Leaderboard", LoadSceneMode.Single);

            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogError($"Không thể chuyển Scene: {status}");
            }
        }
    }

    // Hàm Rpc này bắt tất cả máy con phải khóa nhân vật lại, không cho chết hay chạy nhảy nữa
    [ClientRpc]
    void FreezeAllPlayersClientRpc()
    {
        PlayerNetwork[] players = FindObjectsOfType<PlayerNetwork>();
        foreach (var p in players)
        {
            // Bứt ra khỏi mớ bòng bong của Scene
            p.transform.SetParent(null);

            // --- BÙA HỘ MỆNH CHỐNG CHÉM NHẦM Ở ĐÂY ---
            // Lệnh này giúp nhân vật không bị xóa khi Scene cũ biến mất,
            // nó sẽ ngoan ngoãn chờ lệnh Despawn hợp lệ từ Server bay tới.
            DontDestroyOnLoad(p.gameObject);

            if (p.rb != null)
            {
                p.rb.velocity = Vector2.zero;
                p.rb.simulated = false;
            }

            p.enabled = false;
        }
    }
}