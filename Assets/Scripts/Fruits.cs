using UnityEngine;
using Unity.Netcode;

public class Fruits : NetworkBehaviour
{
    [SerializeField] private int scoreValue = 10;
    private bool isCollected = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // CHỈ SERVER mới có quyền xóa quả dâu và cộng điểm
        if (!IsServer) return; 

        if (collision.CompareTag("Player") && !isCollected)
        {
            isCollected = true;

            // Cộng điểm vào ScoreManager (Server ghi vào NetworkVariable)
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(scoreValue);
            }

            // Xóa quả dâu trên toàn mạng bằng Despawn
            var netObj = GetComponent<NetworkObject>();
            if (netObj != null && netObj.IsSpawned)
            {
                netObj.Despawn(); 
            }
        }
    }
}