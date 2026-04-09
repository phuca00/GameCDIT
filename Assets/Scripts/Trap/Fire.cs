using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public Animator anim;

    public float switchTime = 5f;  // 5 giây đổi trạng thái
    private float timer = 0f;

    private bool isHit = false;    // false = on, true = hit

    public int damage = 1;
    public float topOffset = 0.3f; // khoảng lệch để nhận biết "từ trên xuống"

    void Start()
    {
        anim.Play("on");
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= switchTime)
        {
            timer = 0f;

            if (isHit)
            {
                anim.Play("on");
                isHit = false;
            }
            else
            {
                anim.Play("hit");
                isHit = true;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        // chỉ gây damage khi đang ở animation ON
        if (!IsOnAnimation()) return;

        Transform player = collision.collider.transform;

        // kiểm tra player có ở TRÊN đầu fire không
        if (player.position.y > transform.position.y + topOffset)
        {
            collision.collider.GetComponent<PlayerHealth>()?.TakeDamage(damage, Vector2.zero);
        }
    }

    private bool IsOnAnimation()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        return info.IsName("on");   // tên phải giống 100%
    }
}
