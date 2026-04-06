using UnityEngine;
using Unity.Netcode;

public class Fruits : NetworkBehaviour
{
    [SerializeField] private int scoreValue = 1;

    private bool isCollected = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // chỉ server xử lý
        if (!IsServer) return;

        // tránh ăn 2 lần khi nhiều player chạm cùng lúc
        if (isCollected) return;

        if (!collision.CompareTag("Player")) return;

        isCollected = true;

        // cộng điểm
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue);
        }

        // gọi hiệu ứng cho tất cả client (nếu có)
        PlayCollectEffectClientRpc();

        // xoá object trên toàn network
        NetworkObject.Despawn(true);
    }

    [ClientRpc]
    void PlayCollectEffectClientRpc()
    {
        // bạn có thể thêm:
        // - sound
        // - particle effect
        // - floating text

        // ví dụ:
        // AudioManager.instance.PlayFruit();
    }
}