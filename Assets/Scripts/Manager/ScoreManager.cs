using UnityEngine;
using Unity.Netcode;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance;

    // Sync điểm cho tất cả client
    public NetworkVariable<int> score = new NetworkVariable<int>(0);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // chỉ server được phép cộng điểm
    public void AddScore(int amount)
    {
        if (!IsServer) return;

        score.Value += amount;
    }

    // reset điểm (server)
    public void ResetScore()
    {
        if (!IsServer) return;

        score.Value = 0;
    }
}