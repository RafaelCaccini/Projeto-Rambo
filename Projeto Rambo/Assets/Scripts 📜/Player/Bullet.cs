using UnityEngine;

public class Bullet : MonoBehaviour
{
    // A vari�vel p�blica "lifeTime" pode ser ajustada no Inspector do Unity
    public float lifeTime = 10f;

    // O m�todo Start � chamado uma vez no in�cio da vida do objeto
    void Start()
    {
        // Chama o m�todo "Destroy" ap�s "lifeTime" segundos
        Destroy(gameObject, lifeTime);
    }

    // Detecta a colis�o com outra caixa de colis�o
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider)
        {
            // Destroi o tiro imediatamente ao colidir com qualquer coisa
            Destroy(gameObject);
        }
    }
}