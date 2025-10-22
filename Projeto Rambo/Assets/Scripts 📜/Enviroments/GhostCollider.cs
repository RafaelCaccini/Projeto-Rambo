using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class IgnoreGroundCollision : MonoBehaviour
{
    private Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            Physics2D.IgnoreCollision(col, collision.collider, true);
            Debug.Log($"{name} ignorou colisão com {collision.collider.name}");
        }
    }
}
