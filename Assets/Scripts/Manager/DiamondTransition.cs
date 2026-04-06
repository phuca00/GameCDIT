using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
// Cần thêm using System; nếu class Checkpoint.cs định nghĩa sự kiện
// public static event Action OnCheckpointActivated;

// Đổi tên class để phản ánh chức năng mới: Quản lý chuyển cảnh
public class SceneTransitionManager : MonoBehaviour
{
    // Khai báo các đối tượng chuyển cảnh
    [SerializeField] private GameObject _startingSceneTransition;
    [SerializeField] private GameObject _endingSceneTransition;

    // Biến để lưu trữ tên Scene tiếp theo
    [SerializeField] private string _nextSceneName; // Đặt giá trị mặc định
    
    [SerializeField] private float _endingTransitionDelay = 1.5f; // Thời gian đợi sau khi bật hiệu ứng kết thúc

    // --- LOGIC HIỆU ỨNG BẮT ĐẦU ---

    private void Start()
    {
        // 1. Kích hoạt hiệu ứng mở đầu
        _startingSceneTransition.SetActive(true);
        // 2. Tắt hiệu ứng mở đầu sau 5 giây
        StartCoroutine(DisableTransitionAfterTime(_startingSceneTransition, 5f));
    }

    private IEnumerator DisableTransitionAfterTime(GameObject transitionObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        transitionObject.SetActive(false);
    }
    
    // --- LOGIC LẮNG NGHE CHECKPOINT VÀ CHUYỂN SCENE ---

    private void OnEnable()
    {
        // Đăng ký (Subscribe) lắng nghe sự kiện từ class Checkpoint
        // Đảm bảo Checkpoint.OnCheckpointActivated là public static
        //Checkpoint.OnCheckpointActivated += StartTransition;
    }

    private void OnDisable()
    {
        // Hủy đăng ký (Unsubscribe) khi GameObject bị vô hiệu hóa
        //Checkpoint.OnCheckpointActivated -= StartTransition;
    }

    // Hàm được gọi khi Checkpoint kích hoạt sự kiện
    private void StartTransition()
    {
        // 1. Kích hoạt hiệu ứng chuyển cảnh kết thúc
        _endingSceneTransition.SetActive(true);
        
        // 2. Chờ đợi hết thời gian của animation kết thúc rồi tải scene
        StartCoroutine(LoadNextLevelAfterDelay(_endingTransitionDelay));
    }

    private IEnumerator LoadNextLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Tải Scene mới
        SceneManager.LoadScene(_nextSceneName); 
    }
}