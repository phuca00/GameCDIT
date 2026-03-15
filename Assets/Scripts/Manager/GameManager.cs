using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject retryUI;

    void Awake()
    {
        instance = this;
    }

    public void PlayerDied()
    {
        Invoke(nameof(ShowRetryUI), 0.5f); // đợi animation chết xong
    }

    void ShowRetryUI()
    {
        retryUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ReplayLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
