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

    [Header("Delay")] [SerializeField] private float leaderboardTime = 2f;

    private NetworkVariable<float> currentTime = new NetworkVariable<float>();
    private bool hasEnded = false;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void FindReferencesTimeText()
    {
        if (timeText == null) timeText = GameObject.Find("Time").GetComponent<TMP_Text>();
    }

public override void OnNetworkSpawn()
    {
        // 🔥 CHỈ HOST (server) set thời gian ban đầu
        if (IsServer)
        {
            currentTime.Value = startTime;
        }
    }

    private void Update()
    {
        // 🔥 CHỈ SERVER đếm ngược
        if (IsServer && !hasEnded)
        {
            currentTime.Value -= Time.deltaTime;

            if (currentTime.Value <= 0f)
            {
                currentTime.Value = 0f;
                hasEnded = true;

                StartCoroutine(EndGameFlow());
            }
        }

        // 🔥 TẤT CẢ CLIENT chỉ hiển thị
        UpdateUI();
    }

    void UpdateUI()
    {
        if (timeText == null) return;

        int seconds = Mathf.CeilToInt(currentTime.Value);
        timeText.text = seconds.ToString();
    }

    IEnumerator EndGameFlow()
    {
        Debug.Log("Hết giờ - Server xử lý");

        string currentScene = SceneManager.GetActiveScene().name;

        // lưu lại level hiện tại
        PlayerPrefs.SetString("LastLevel", currentScene);

        // 👉 CHỈ load leaderboard
        NetworkManager.Singleton.SceneManager.LoadScene("Leaderboard", LoadSceneMode.Single);

        yield break; // 🔥 DỪNG tại đây
    

    }
}