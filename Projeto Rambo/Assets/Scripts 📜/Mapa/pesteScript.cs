using UnityEngine;
using System.Collections;

public class pesteScript : MonoBehaviour
{
    [Header("Configuração de Movimento")]
    public float velocidade = 2f;
    private bool movimentoAtivo = true;

    [Header("Configuração de Colisão")]
    public string tagPlayer = "Player";
    public string tagEnemy = "Enemy";

    private Collider2D meuCollider;

    void Start()
    {
        meuCollider = GetComponent<Collider2D>();
        if (meuCollider == null)
        {
            Debug.LogError("pesteScript precisa de um Collider2D.");
            enabled = false;
        }
    }

    void Update()
    {
        if (movimentoAtivo)
        {
            transform.Translate(Vector2.right * velocidade * Time.deltaTime);
        }
    }

    // Método público que será chamado pelo outro script para parar a peste
    public void PararMovimento()
    {
        movimentoAtivo = false;
        Debug.Log("Movimento da peste foi parado por um trigger externo.");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(tagPlayer) || collision.gameObject.CompareTag(tagEnemy))
        {
            Physics2D.IgnoreCollision(meuCollider, collision.collider, true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(tagPlayer) || collision.gameObject.CompareTag(tagEnemy))
        {
            Physics2D.IgnoreCollision(meuCollider, collision.collider, false);
        }
    }
}