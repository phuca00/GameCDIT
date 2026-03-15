using UnityEngine;

public class BlockDetector : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D col)
    {
        // player bật vào từ dưới
        if (col.collider.CompareTag("Player"))
        {
            if (col.contacts[0].normal.y < -0.3f) // player đánh lên từ dưới
            {
                GetComponent<BreakBlock>().Break();
            }
        }
    }
}
