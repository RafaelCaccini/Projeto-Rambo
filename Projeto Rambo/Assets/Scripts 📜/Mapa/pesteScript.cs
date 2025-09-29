using UnityEngine;
using System.Collections;

public class pesteScript : MonoBehaviour
{
    [Header("Configura��o de Movimento")]
    public float velocidade = 2f; // velocidade da peste
    private bool movimentoAtivo = true; // controla se a peste se move

    [Header("Configura��o de Colis�o")]
    public string tagPlayer = "Player"; // tag do jogador
    public string tagEnemy = "Enemy";   // tag dos inimigos

    private Collider2D meuCollider; // refer�ncia ao pr�prio collider

    void Start()
    {
        // pega o collider do objeto
        meuCollider = GetComponent<Collider2D>();
        if (meuCollider == null)
        {
            Debug.LogError("pesteScript precisa de um Collider2D."); // avisa se n�o tiver collider
            enabled = false;
        }
    }

    void Update()
    {
        // se o movimento estiver ativo, move a peste para a direita
        if (movimentoAtivo)
        {
            transform.Translate(Vector2.right * velocidade * Time.deltaTime);
        }
    }

    // fun��o que pode ser chamada de fora pra parar a peste
    public void PararMovimento()
    {
        movimentoAtivo = false;
        Debug.Log("Movimento da peste foi parado por um trigger externo.");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // se bater no jogador ou inimigo, ignora a colis�o f�sica
        if (collision.gameObject.CompareTag(tagPlayer) || collision.gameObject.CompareTag(tagEnemy))
        {
            Physics2D.IgnoreCollision(meuCollider, collision.collider, true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // quando sai da colis�o com o jogador ou inimigo, volta a colidir normalmente
        if (collision.gameObject.CompareTag(tagPlayer) || collision.gameObject.CompareTag(tagEnemy))
        {
            Physics2D.IgnoreCollision(meuCollider, collision.collider, false);
        }
    }
}
