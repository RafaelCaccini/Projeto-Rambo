using UnityEngine;

public class Bullet : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col) //Detecta a colis�o com outra caixa de colis�o. Mesmo sem ser isTrigger.
    {
        if (col.collider) // aplica apenas se o objeto tiver a tag Danger.
        {
            Destroy(gameObject); // Destroi o tiro ao colidir com qualquer coisa
        }
    }
}
