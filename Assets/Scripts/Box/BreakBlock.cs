using UnityEngine;

public class BreakBlock : MonoBehaviour
{
    public GameObject piecePrefab;
    public Sprite[] pieceSprites;   // gán 4 sprite vào đây
    public float force = 5f;        // lực nổ ra
    public float torque = 5f;       // xoay nhẹ

    public void Break()
    {
        // Spawn 4 mảnh
        for (int i = 0; i < 4; i++)
        {
            GameObject piece = Instantiate(piecePrefab, transform.position, Quaternion.identity);

            // sprite mảnh
            piece.GetComponent<SpriteRenderer>().sprite = pieceSprites[i];

            Rigidbody2D rb = piece.GetComponent<Rigidbody2D>();

            // hướng bắn ra
            Vector2 dir = new Vector2(
                Random.Range(-1f, 1f),
                Random.Range(0.2f, 1f)
            ).normalized;

            // add force
            rb.AddForce(dir * force, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-torque, torque), ForceMode2D.Impulse);
        }

        Destroy(gameObject); // block biến mất
    }
}
