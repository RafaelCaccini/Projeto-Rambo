using UnityEngine;

public class ColisaoAtravessavel : MonoBehaviour
{
    private Collider2D meuCollider;

    [Header("Configura��o de Colis�o")]
    public string tagPlayer = "Player"; // quem pode atravessar
    public string tagEnemy = "Enemy";   // quem tamb�m pode atravessar

    void Start()
    {
        meuCollider = GetComponent<Collider2D>();

        if (meuCollider == null)
        {
            Debug.LogError("Esse objeto precisa de Collider2D!");
            enabled = false;
            return;
        }

        // ignora colis�o f�sica no come�o, mas ainda detecta eventos
        ConfigurarIgnorarColisao(tagPlayer);
        ConfigurarIgnorarColisao(tagEnemy);
    }

    // configura para ignorar colis�o com todos os objetos de uma tag
    private void ConfigurarIgnorarColisao(string tagParaIgnorar)
    {
        GameObject[] objetosComTag = GameObject.FindGameObjectsWithTag(tagParaIgnorar);

        foreach (GameObject obj in objetosComTag)
        {
            Collider2D colliderDoOutro = obj.GetComponent<Collider2D>();

            if (colliderDoOutro != null)
            {
                // o objeto atravessa, mas OnCollisionEnter2D ainda funciona
                Physics2D.IgnoreCollision(meuCollider, colliderDoOutro, true);
            }
        }
    }

    // detecta colis�o mesmo quando atravessa
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Colis�o detectada com: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag(tagPlayer) || collision.gameObject.CompareTag(tagEnemy))
        {
            // evento l�gico mesmo sem f�sica
            Debug.Log($"Colis�o l�gica com {collision.gameObject.tag} detectada!");
        }
        else
        {
            // colis�o normal com outros objetos
            Debug.Log($"Colis�o s�lida com: {collision.gameObject.name}");
        }
    }
}
