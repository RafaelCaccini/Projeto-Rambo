using UnityEngine;

public class Espartano : MonoBehaviour
{
    [Header("Referências")]
    public Transform jogador;              // arrasta o Player
    public GameObject escudoPrefab;        // objeto invisível (escudo)
    public GameObject dangerPrefab;        // prefab com tag "Danger"
    public Transform spawnEscudo;          // ponto de spawn na frente
    public Transform spawnDanger;          // ponto de spawn do ataque

    [Header("Movimento")]
    public float velocidade = 2f;
    public float distanciaAtaque = 1.5f;
    public float tempoEntreAtaques = 1.2f; // cooldown entre ataques

    [Header("Detecção")]
    public float raioDeteccao = 6f; // alcance em que começa a perseguir
    private bool jogadorNoAlcance = false;

    [Header("Flip")]
    public float tempoParaVirar = 0.5f; // delay ao virar
    private bool olhandoDireita = true;
    private float tempoUltimaTroca = 0f;

    [Header("Escudo")]
    private GameObject escudoAtual;

    private Rigidbody2D rb;
    private Animator anim;
    private float tempoUltimoAtaque = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Spawna o escudo inicial
        if (escudoPrefab != null && spawnEscudo != null)
        {
            escudoAtual = Instantiate(escudoPrefab, spawnEscudo.position, Quaternion.identity, transform);
        }
    }

    void Update()
    {
        if (jogador == null) return;

        // Checa se o player está dentro do raio de detecção
        float distancia = Vector2.Distance(transform.position, jogador.position);
        jogadorNoAlcance = distancia <= raioDeteccao;

        if (!jogadorNoAlcance)
        {
            Parar();
            anim?.SetBool("Andando", false);
            return;
        }

        if (distancia > distanciaAtaque)
        {
            Mover();
            //anim?.SetBool("Andando", true);
        }
        else
        {
            Parar();
            //anim?.SetBool("Andando", false);

            if (Time.time >= tempoUltimoAtaque + tempoEntreAtaques)
            {
                Atacar();
                tempoUltimoAtaque = Time.time;
            }
        }

        FlipDelay();
    }

    private void Mover()
    {
        Vector2 direcao = (jogador.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direcao.x * velocidade, rb.linearVelocity.y);

        // mantém escudo na frente
        if (escudoAtual != null && spawnEscudo != null)
        {
            escudoAtual.transform.position = spawnEscudo.position;
        }
    }

    private void Parar()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    private void Atacar()
    {
        if (dangerPrefab != null && spawnDanger != null)
        {
            // cria a hitbox na frente
            GameObject hitbox = Instantiate(dangerPrefab, spawnDanger.position, Quaternion.identity, transform);

            // some rapidão (0.2s por exemplo)
            Destroy(hitbox, 0.2f);
        }
    }


    private void FlipDelay()
    {
        bool deveOlharDireita = jogador.position.x > transform.position.x;

        if (deveOlharDireita != olhandoDireita && Time.time - tempoUltimaTroca > tempoParaVirar)
        {
            olhandoDireita = deveOlharDireita;
            tempoUltimaTroca = Time.time;

            Vector3 escala = transform.localScale;
            escala.x *= -1; // inverte tudo, inclusive filhos
            transform.localScale = escala;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Gizmo do raio de detecção
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raioDeteccao);

        // Gizmo do alcance de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
    }
}
