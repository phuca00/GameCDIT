using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private float startTime = 60f;
    [SerializeField] private float transitionDelay = 1f;

    private float currentTime;
    private bool isRunning = true;

    private void Start()
    {
        currentTime = startTime;
        UpdateUI();
    }

    private void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isRunning = false;
            StartCoroutine(EndGame());
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        int seconds = Mathf.CeilToInt(currentTime);
        timeText.text = seconds.ToString();
    }

    IEnumerator EndGame()
    {
        // LẤY ĐIỂM TỪ SCORE
        int finalScore = Score.instance.GetScore();

        string sceneName = SceneManager.GetActiveScene().name;

        // LƯU SCENE + ĐIỂM
        PlayerPrefs.SetString("LastScene", sceneName);
        PlayerPrefs.SetInt(sceneName + "_Score", finalScore);

        yield return new WaitForSeconds(transitionDelay);

        SceneManager.LoadScene("Leaderboard");
    }
}