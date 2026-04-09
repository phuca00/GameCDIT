using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode; // Bắt buộc phải có Netcode

public class SceneTransitionManager : MonoBehaviour
{
    [SerializeField] private GameObject _startingSceneTransition;
    [SerializeField] private GameObject _endingSceneTransition;
    [SerializeField] private string _nextSceneName; 
    [SerializeField] private float _endingTransitionDelay = 1.5f; 

    private void Start()
    {
        _startingSceneTransition.SetActive(true);
        StartCoroutine(DisableTransitionAfterTime(_startingSceneTransition, 5f));
    }

    private IEnumerator DisableTransitionAfterTime(GameObject transitionObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        transitionObject.SetActive(false);
    }
    
    // Gắn hàm này vào Cửa / Checkpoint cuối màn
    public void StartTransition()
    {
        // 1. Máy nào chạm cửa thì máy đó tự bật hiệu ứng đen xì
        _endingSceneTransition.SetActive(true);
        
        // Báo cho các máy khác cũng bật hiệu ứng (Nếu ông có script Network riêng cho cửa, còn không thì bỏ qua)
        
        // 2. Chờ đợi hết thời gian của animation kết thúc rồi tải scene
        StartCoroutine(LoadNextLevelAfterDelay(_endingTransitionDelay));
    }

    private IEnumerator LoadNextLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // QUAN TRỌNG NHẤT: Chỉ Server mới có quyền chuyển Scene
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(_nextSceneName, LoadSceneMode.Single);
        }
    }
}