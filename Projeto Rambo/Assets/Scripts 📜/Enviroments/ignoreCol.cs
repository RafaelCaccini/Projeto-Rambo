using UnityEngine;

public class ColisaoAtravessavel : MonoBehaviour
{
    private Collider2D meuCollider;

    [Header("Configura��o de Colis�o")]
    [Tooltip("Tags que podem atravessar este objeto, mas cujas colis�es devem ser detectadas.")]
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

        // 1. Configura para ignorar a colis�o f�sica no in�cio
        ConfigurarIgnorarColisao(tagPlayer);
        ConfigurarIgnorarColisao(tagEnemy);
    }

    /// <summary>
    /// Encontra todos os objetos na cena com a tag e configura para ignorar a colis�o f�sica.
    /// </summary>
    private void ConfigurarIgnorarColisao(string tagParaIgnorar)
    {
        GameObject[] objetosComTag = GameObject.FindGameObjectsWithTag(tagParaIgnorar);

        foreach (GameObject obj in objetosComTag)
        {
            Collider2D colliderDoOutro = obj.GetComponent<Collider2D>();

            if (colliderDoOutro != null)
            {
                // Este � o comando principal: A Unity N�O calcula a for�a de colis�o
                // permitindo que o objeto atravesse, mas o evento de colis�o AINDA � gerado.
                Physics2D.IgnoreCollision(meuCollider, colliderDoOutro, true);
            }
        }
    }

    // AQUI � O PONTO CHAVE: O evento OnCollisionEnter2D ser� chamado!
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Ponto 1: Detecta a colis�o
        Debug.Log("Colis�o detectada com: " + collision.gameObject.name);

        // Ponto 2: Verifica se � o Player ou Enemy (para que voc� possa adicionar l�gica especial)
        if (collision.gameObject.CompareTag(tagPlayer) || collision.gameObject.CompareTag(tagEnemy))
        {
            // Adicione a l�gica que voc� precisa para o momento em que o Player/Enemy "bate" nele:
            // Ex: Tocar um som, aplicar dano, etc.
            Debug.Log($"Colis�o l�gica com {collision.gameObject.tag} detectada! (Embora a f�sica tenha sido ignorada)");
        }
        else
        {
            // L�gica para objetos que N�O s�o o Player ou Enemy (Ex: Inimigos que colidem normalmente)
            // Note que eles ainda ser�o impedidos de atravessar por padr�o, j� que a colis�o deles n�o � ignorada.
            Debug.Log($"Colis�o s�lida com: {collision.gameObject.name}");
        }
    }

    // Voc� tamb�m pode usar OnCollisionStay2D para verificar enquanto est�o em contato
    // void OnCollisionStay2D(Collision2D collision)
    // {
    //     // L�gica para quando o objeto est� atravessando ou em contato.
    // }
}