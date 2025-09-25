using UnityEngine;

public class ColisaoAtravessavel : MonoBehaviour
{
    private Collider2D meuCollider;

    [Header("Configuração de Colisão")]
    [Tooltip("Tags que podem atravessar este objeto, mas cujas colisões devem ser detectadas.")]
    public string tagPlayer = "Player";
    public string tagEnemy = "Enemy";

    void Start()
    {
        meuCollider = GetComponent<Collider2D>();

        if (meuCollider == null)
        {
            Debug.LogError("Objeto '" + gameObject.name + "' precisa de um Collider2D para usar este script.");
            enabled = false;
            return;
        }

        // 1. Configura para ignorar a colisão física no início
        ConfigurarIgnorarColisao(tagPlayer);
        ConfigurarIgnorarColisao(tagEnemy);
    }

    /// <summary>
    /// Encontra todos os objetos na cena com a tag e configura para ignorar a colisão física.
    /// </summary>
    private void ConfigurarIgnorarColisao(string tagParaIgnorar)
    {
        GameObject[] objetosComTag = GameObject.FindGameObjectsWithTag(tagParaIgnorar);

        foreach (GameObject obj in objetosComTag)
        {
            Collider2D colliderDoOutro = obj.GetComponent<Collider2D>();

            if (colliderDoOutro != null)
            {
                // Este é o comando principal: A Unity NÃO calcula a força de colisão
                // permitindo que o objeto atravesse, mas o evento de colisão AINDA é gerado.
                Physics2D.IgnoreCollision(meuCollider, colliderDoOutro, true);
            }
        }
    }

    // AQUI É O PONTO CHAVE: O evento OnCollisionEnter2D será chamado!
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Ponto 1: Detecta a colisão
        Debug.Log("Colisão detectada com: " + collision.gameObject.name);

        // Ponto 2: Verifica se é o Player ou Enemy (para que você possa adicionar lógica especial)
        if (collision.gameObject.CompareTag(tagPlayer) || collision.gameObject.CompareTag(tagEnemy))
        {
            // Adicione a lógica que você precisa para o momento em que o Player/Enemy "bate" nele:
            // Ex: Tocar um som, aplicar dano, etc.
            Debug.Log($"Colisão lógica com {collision.gameObject.tag} detectada! (Embora a física tenha sido ignorada)");
        }
        else
        {
            // Lógica para objetos que NÃO são o Player ou Enemy (Ex: Inimigos que colidem normalmente)
            // Note que eles ainda serão impedidos de atravessar por padrão, já que a colisão deles não é ignorada.
            Debug.Log($"Colisão sólida com: {collision.gameObject.name}");
        }
    }

    // Você também pode usar OnCollisionStay2D para verificar enquanto estão em contato
    // void OnCollisionStay2D(Collision2D collision)
    // {
    //     // Lógica para quando o objeto está atravessando ou em contato.
    // }
}