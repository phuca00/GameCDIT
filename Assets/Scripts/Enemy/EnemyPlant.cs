using UnityEngine;
using System.Collections;  

public class EnemyPlant : MonoBehaviour
{
    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Attack Settings")]
    public float attackCooldown = 5f;
    private float attackTimer;

    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0;
            anim.SetTrigger("Attack");   // animation Attack
        }
    }

    // Hàm này được gọi ở cuối animation Attack (Animation Event)
    public void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}