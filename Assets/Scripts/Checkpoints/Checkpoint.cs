using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections; // Cần thiết cho Coroutine

public class Checkpoint : MonoBehaviour
{
    // Event để thông báo cho SceneTransitionManager
    public static event Action OnCheckpointActivated;

    [SerializeField] private Animator animator;
    [SerializeField] private string animationName = "checkpoint";
    
    // 💡 KHAI BÁO MỚI: Biến để điều chỉnh thời gian delay trước khi chuyển scene
    [SerializeField] private float _transitionDelay = 5f; 

    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Tránh kích hoạt nhiều lần
        if (activated) return; 
        
        // Chỉ Player mới chạy
        if (!collision.CompareTag("Player")) return;

        activated = true;
        AudioManager.instance.PlayCheckpoint();

        // --- HÀNH ĐỘNG CỦA CHECKPOINT (Chạy ngay lập tức) ---

        // 1. Lưu tiến trình
        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Completed", 1);

        // 2. Chạy animation checkpoint
        if (animator != null)
            animator.Play(animationName);
        
        // 3. Bắt đầu Coroutine để delay 5s rồi bắn sự kiện
        StartCoroutine(ActivateCheckpointWithDelay(_transitionDelay));
    }

    /// <summary>
    /// Coroutine chờ đợi X giây sau đó kích hoạt sự kiện chuyển cảnh.
    /// </summary>
    private IEnumerator ActivateCheckpointWithDelay(float delay)
    {
        // Chờ đợi trong khoảng thời gian đã chỉ định (ví dụ: 5 giây)
        yield return new WaitForSeconds(delay); 

        // 4. Thông báo cho hệ thống chuyển Scene (sau khi delay)
        OnCheckpointActivated?.Invoke();
    }
}