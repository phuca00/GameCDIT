using UnityEngine;
using Unity.Netcode;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance;
    
    public NetworkVariable<int> score = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        score.OnValueChanged += (oldVal, newVal) => {
            UpdateScoreUI(newVal);
        };
        // Cập nhật điểm cho Client vừa mới vào phòng
        UpdateScoreUI(score.Value);
    }

    void UpdateScoreUI(int value)
    {
        if (Score.instance != null)
        {
            // Set thẳng text cho nhanh (Nếu Score.instance có hàm SetScore)
            // Nếu không có, dùng logic cũ của ông:
            int currentUIScore = Score.instance.GetScore();
            Score.instance.AddScore(value - currentUIScore);
        }
    }

    public void AddScore(int amount)
    {
        if (IsServer) 
        {
            score.Value += amount; // Server thì tự cộng luôn
        }
        else 
        {
            RequestAddScoreServerRpc(amount); // Client thì nhờ Server cộng hộ
        }
    }

    // RequireOwnership = false cho phép AI CŨNG CÓ THỂ gọi hàm này (Rất quan trọng cho Client)
    [ServerRpc(RequireOwnership = false)]
    void RequestAddScoreServerRpc(int amount)
    {
        score.Value += amount;
    }
}