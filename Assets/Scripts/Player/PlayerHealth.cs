using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class PlayerHealth : NetworkBehaviour 
{
    private Vector3 startPosition;
    public Animator animator;
    public Rigidbody2D rb;

    public override void OnNetworkSpawn()
    {
        startPosition = transform.position;
    }

    // Hàm này sẽ được gọi khi chạm bẫy
    public void TakeDamage(int dmg, Vector2 knockback)
    {
        // DO CLIENT ĐANG LÀM CHỦ VẬT LÝ -> CLIENT TỰ XỬ LÝ CHẾT
        if (IsOwner) 
        {
            StartCoroutine(RespawnFlow());
            // Báo cho Server và các máy khác biết để chúng nó phát Animation chết
            NotifyDeathServerRpc(); 
        }
    }

    [ServerRpc]
    void NotifyDeathServerRpc() => NotifyDeathClientRpc();

    [ClientRpc]
    void NotifyDeathClientRpc()
    {
        // Máy owner đã tự chạy RespawnFlow() rồi, nên các máy khác mới cần chạy để đồng bộ hình ảnh
        if (!IsOwner) StartCoroutine(RespawnFlow());
    }

    IEnumerator RespawnFlow()
    {
        if (animator != null) animator.SetTrigger("death");
        rb.velocity = Vector2.zero;

        var networkMove = GetComponent<PlayerNetwork>();
        if (networkMove != null) networkMove.enabled = false;

        // Tắt va chạm tạm thời để không bị chết liên tục
        rb.simulated = false; 

        yield return new WaitForSeconds(0.6f);

        // Chỉ chủ nhân mới tự kéo vị trí về
        if (IsOwner) transform.position = startPosition;

        if (networkMove != null) networkMove.enabled = true;
        if (animator != null) animator.Rebind();
        
        // Bật lại va chạm
        rb.simulated = true; 

        if (IsOwner && GameManager.instance != null)
        {
            // GameManager.instance.PlayerDied();
        }
    }
    public void SetNewSpawnPoint(Vector3 newPos)
    {
        startPosition = newPos;
    }
}