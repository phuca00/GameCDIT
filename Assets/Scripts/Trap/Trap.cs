using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public int damage = 1;
    public float knockbackForce = 5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.collider.GetComponent<Rigidbody2D>();
            PlayerHealth health = collision.collider.GetComponent<PlayerHealth>();

            Vector2 dir = (collision.transform.position - transform.position).normalized;
            Vector2 knockback = new Vector2(dir.x * knockbackForce, 5f);

            health.TakeDamage(damage, knockback);
        }
    }
}
