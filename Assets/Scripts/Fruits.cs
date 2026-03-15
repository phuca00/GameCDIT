using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruits : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                // khi ăn fruit
                AudioManager.instance.PlayFruit();

                // Ẩn fruit 
                gameObject.SetActive(false);
            }
        }
}
