using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CountdownTimer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("Time Settings")]
    [SerializeField] private float startTime = 60f;

    [Header("Scene Transition")]
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

    private System.Collections.IEnumerator EndGame()
    {
        Debug.Log("Hết giờ!");

        yield return new WaitForSeconds(transitionDelay);
        // chuyển sang scene tiếp theo
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}