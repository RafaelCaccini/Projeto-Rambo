using UnityEngine;

public class ColisaoAtravessavel : MonoBehaviour
{
    private Collider2D meuCollider;

    [Header("Configuração de Colisão")]
    public string tagPlayer = "Player"; // quem pode atravessar
    public string tagEnemy = "Enemy";   // quem também pode atravessar

    void Start()
    {
        meuCollider = GetComponent<Collider2D>();

        if (meuCollider == null)
        {
            Debug.LogError("Esse objeto precisa de Collider2D!");
            enabled = false;
            return;
        }

        // ignora colisão física no começo, mas ainda detecta eventos
        ConfigurarIgnorarColisao(tagPlayer);
        ConfigurarIgnorarColisao(tagEnemy);
    }

    // configura para ignorar colisão com todos os objetos de uma tag
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

    // detecta colisão mesmo quando atravessa
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Colisão detectada com: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag(tagPlayer) || collision.gameObject.CompareTag(tagEnemy))
        {
            // evento lógico mesmo sem física
            Debug.Log($"Colisão lógica com {collision.gameObject.tag} detectada!");
        }
        else
        {
            // colisão normal com outros objetos
            Debug.Log($"Colisão sólida com: {collision.gameObject.name}");
        }
    }
}
