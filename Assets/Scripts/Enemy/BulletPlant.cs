using UnityEngine;

public class BulletPlant : MonoBehaviour
{
    public float speed = 4f;
    public int damage = 1;

    void Update()
    {
        // Bay sang trái
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<PlayerHealth>()?.TakeDamage(damage, Vector2.zero);
        }

        gameObject.SetActive(false);
    }
}