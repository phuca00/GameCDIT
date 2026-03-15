using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public int score = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Lấy điểm đã lưu (nếu có)
        score = PlayerPrefs.GetInt("TotalScore", 0);
    }

    public void AddScore(int amount)
    {
        score += amount;
        SaveScore();
    }

    public void SaveScore()
    {
        PlayerPrefs.SetInt("TotalScore", score);
        PlayerPrefs.Save();
    }

    public void ResetScore()
    {
        score = 0;
        SaveScore();
    }

}
