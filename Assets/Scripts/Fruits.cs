using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruits : MonoBehaviour
{
    private SpawnItem spawnSystem;

    [Header("Score Settings")]
    [SerializeField] private int scoreValue = 1;   // điểm của quả này

    private void Start()
    {
        spawnSystem = FindObjectOfType<SpawnItem>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // chỉ xử lý khi player chạm vào
        if (!collision.CompareTag("Player")) return;

        // phát âm thanh ăn quả
        if (AudioManager.instance != null)
            AudioManager.instance.PlayFruit();

        // cộng điểm
        if (Score.instance != null)
            Score.instance.AddScore(scoreValue);

        // báo cho hệ thống spawn biết đã nhặt item
        if (spawnSystem != null)
            spawnSystem.ItemCollected();

        // hủy quả
        Destroy(gameObject);
    }
}