using UnityEngine;

public class BreakPiece : MonoBehaviour
{
    public float lifeTime = 1.2f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
