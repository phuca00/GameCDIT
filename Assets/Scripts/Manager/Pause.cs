using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Pause : MonoBehaviour
{
    [SerializeField] public static bool inputLocked = false;

    public GameObject pauseUI;   // Dashboard-Pause

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0f;

        inputLocked = true;   // khóa input
    }

    public void ResumeGame()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        inputLocked = false;  // mở lại input
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        inputLocked = false; // reset tốc độ game nếu đang pause
        SceneManager.LoadScene("Menu");
    }

    public void LoadSetting()
    {
        Time.timeScale = 1f;
        inputLocked = false;
        SceneManager.LoadScene("Setting");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        inputLocked = false;
        SceneManager.LoadScene("Menu");
    }
}
