using UnityEngine;

public class Projetil : MonoBehaviour
{

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Destroi o projétil quando encostar em QUALQUER coisa
        Destroy(gameObject);
    }
}
