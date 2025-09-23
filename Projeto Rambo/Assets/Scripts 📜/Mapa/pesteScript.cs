using UnityEngine;

public class pesteScript : MonoBehaviour
{
    [Header("Configura��o de Movimento")]
    public float velocidade = 2f; // Velocidade de movimento para a direita

    [Header("Configura��o de Colis�o")]
    public string tagPlayer = "Player";
    public string tagEnemy = "Enemy";

    private Collider2D meuCollider;
    private int objetosNaPeste = 0;

    void Start()
    {
        meuCollider = GetComponent<Collider2D>();
        if (meuCollider == null)
        {
            Debug.LogError("PesteScript precisa de um Collider2D no mesmo GameObject.");
            enabled = false; // Desativa o script se n�o houver um collider
        }
    }

    void Update()
    {
        // Movimenta o objeto para a direita
        transform.Translate(Vector2.right * velocidade * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se a colis�o � com um objeto que deve ser "atravessado"
        if (collision.gameObject.CompareTag(tagPlayer) || collision.gameObject.CompareTag(tagEnemy))
        {
            // Desativa a colis�o da Peste para permitir a passagem
            if (meuCollider != null)
            {
                meuCollider.enabled = false;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Reativa a colis�o da Peste quando o objeto que estava passando por ela sai
        if (collision.gameObject.CompareTag(tagPlayer) || collision.gameObject.CompareTag(tagEnemy))
        {
            // Reativa a colis�o apenas se n�o houver outro objeto "passando"
            if (meuCollider != null)
            {
                meuCollider.enabled = true;
            }
        }
    }
}