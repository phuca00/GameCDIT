using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw_vertical : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float min, max;  // giới hạn theo trục Y
    private int x;

    void Start()
    {
        x = 1; // 1 = đi lên, -1 = đi xuống
    }

    void Update()
    {
        // DI CHUYỂN DỌC (Y)
        transform.position += Vector3.up * speed * x * Time.deltaTime;

        if (transform.position.y >= max)
        {
            x = -1;   // đổi hướng đi xuống
        }
        else if (transform.position.y <= min)
        {
            x = 1;    // đổi hướng đi lên
        }
    }
}