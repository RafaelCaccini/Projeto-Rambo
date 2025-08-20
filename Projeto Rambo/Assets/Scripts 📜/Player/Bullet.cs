using UnityEngine;

public class Bullet : MonoBehaviour
{
    // A variável pública "lifeTime" pode ser ajustada no Inspector do Unity
    public float lifeTime = 10f;

    // O método Start é chamado uma vez no início da vida do objeto
    void Start()
    {
        // Chama o método "Destroy" após "lifeTime" segundos
        Destroy(gameObject, lifeTime);
    }

    // Detecta a colisão com outra caixa de colisão
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider)
        {
            // Destroi o tiro imediatamente ao colidir com qualquer coisa
            Destroy(gameObject);
        }
    }
}