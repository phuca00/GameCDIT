using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Checkpoint : MonoBehaviour
{
    public static event System.Action OnCheckpointActivated;

    [Header("Timer")]
    [SerializeField] private float startTime = 60f;
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string animationName = "checkpoint";

    [Header("Transition")]
    [SerializeField] private float _transitionDelay = 1f;

    private float currentTime;
    private bool activated = false;

    private void Start()
    {
        currentTime = startTime;
        UpdateUI();
    }

    private void Update()
    {
        if (activated) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            activated = true;
            StartCoroutine(ActivateCheckpointWithDelay(_transitionDelay));
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (timeText == null) return;

        int seconds = Mathf.CeilToInt(currentTime);
        timeText.text = seconds.ToString();
    }

    private IEnumerator ActivateCheckpointWithDelay(float delay)
    {
        // 🔥 tắt UI ngay khi hết giờ
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            canvas.SetActive(false);
        }

        // phát âm thanh
        if (AudioManager.instance != null)
            AudioManager.instance.PlayCheckpoint();

        // lưu tiến trình
        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Completed", 1);

        // chạy animation
        if (animator != null)
            animator.Play(animationName);

        yield return new WaitForSeconds(delay);

        OnCheckpointActivated?.Invoke();
    }
}