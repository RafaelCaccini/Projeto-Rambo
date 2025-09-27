using UnityEngine;

public class Templario : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float velocidade = 3f;
    private Rigidbody2D rb;

    [Header("Referências")]
    [Tooltip("Arraste e solte o objeto do jogador neste campo.")]
    public Transform jogadorTransform;

    [Header("Configurações de Ataque")]
    public GameObject objetoDeDanoPrefab;
    private bool estaAtacando = false;

    [Header("Detecção do Jogador")]
    [Tooltip("O raio da área de detecção do jogador.")]
    public float raioDeDeteccao = 5f;
    private bool alvoDetectado = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (jogadorTransform == null) return;

        if (!alvoDetectado)
        {
            float distancia = Vector2.Distance(transform.position, jogadorTransform.position);
            if (distancia <= raioDeDeteccao)
            {
                alvoDetectado = true;
                Debug.Log("JOGADOR DETECTADO! O Templário vai começar a se mover.");
            }
        }

        if (alvoDetectado && !estaAtacando)
        {
            rb.AddForce(Vector2.left * velocidade * 5f);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // --- Colisões e Ataque ---

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            rb.linearVelocity = Vector2.zero;
            estaAtacando = true;

            // Chama a função de ataque aqui
            RealizarAtaque();
        }
    }

    // NOVA FUNÇÃO: Lógica de ataque separada
    private void RealizarAtaque()
    {
        if (objetoDeDanoPrefab != null)
        {
            // Debug para confirmar que o ataque foi acionado
            Debug.Log("Realizando Ataque! Objeto de dano instanciado.");

            GameObject objetoDeDano = Instantiate(objetoDeDanoPrefab, transform.position, Quaternion.identity);
            objetoDeDano.tag = "Danger";

            // Destrói o objeto de dano após 0.1 segundos
            Destroy(objetoDeDano, 0.1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raioDeDeteccao);
    }
}