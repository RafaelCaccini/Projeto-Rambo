using UnityEngine;

public class Bullet : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col) //Detecta a colisão com outra caixa de colisão. Mesmo sem ser isTrigger.
    {
        if (col.collider) // aplica apenas se o objeto tiver a tag Danger.
        {
            Destroy(gameObject); // Destroi o tiro ao colidir com qualquer coisa
        }
    }
}
