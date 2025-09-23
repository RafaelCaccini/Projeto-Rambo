using UnityEngine;

public class pesteScript : MonoBehaviour
{
    [Header("Configuração de Movimento")]
    public float velocidade = 2f; // Velocidade de movimento para a direita

    [Header("Configuração de Colisão")]
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
            enabled = false; // Desativa o script se não houver um collider
        }
    }

    void Update()
    {
        // Movimenta o objeto para a direita
        transform.Translate(Vector2.right * velocidade * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se a colisão é com um objeto que deve ser "atravessado"
        if (collision.gameObject.CompareTag(tagPlayer) || collision.gameObject.CompareTag(tagEnemy))
        {
            // Desativa a colisão da Peste para permitir a passagem
            if (meuCollider != null)
            {
                meuCollider.enabled = false;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Reativa a colisão da Peste quando o objeto que estava passando por ela sai
        if (collision.gameObject.CompareTag(tagPlayer) || collision.gameObject.CompareTag(tagEnemy))
        {
            // Reativa a colisão apenas se não houver outro objeto "passando"
            if (meuCollider != null)
            {
                meuCollider.enabled = true;
            }
        }
    }
}