using UnityEngine;

public class Espartano : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [Tooltip("Velocidade do Espartano ao se mover.")]
    public float velocidade = 2.5f;
    [Tooltip("Distância para o Espartano começar a andar em direção ao jogador.")]
    public float raioDeDeteccao = 5f;
    private Rigidbody2D rb;
    private SpriteRenderer sp;

    [Header("Configurações de Virar")]
    [Tooltip("Tempo de atraso para o Espartano virar após o jogador passar para o outro lado.")]
    public float delayParaVirar = 0.5f;
    private bool estaViradoParaDireita = true;
    private float timerVirar;

    [Header("Ataque e Defesa")]
    [Tooltip("Distância que o Espartano precisa estar do jogador para atacar.")]
    public float distanciaDeAtaque = 0.8f;
    [Tooltip("Prefab do objeto de escudo que será gerado.")]
    public GameObject escudoPrefab;
    [Tooltip("Prefab do objeto que causa dano.")]
    public GameObject danoPrefab;
    [Tooltip("Distância que o escudo aparece à frente do Espartano.")]
    public float distanciaEscudo = 0.5f; // NOVO: Distância para o escudo
    private Transform escudoInstanciado;

    [Header("Referências")]
    [Tooltip("Arraste e solte o objeto do jogador aqui.")]
    public Transform jogadorTransform;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        // Bora criar o escudo do Espartano
        if (escudoPrefab != null)
        {
            escudoInstanciado = Instantiate(escudoPrefab, transform).transform;
            escudoInstanciado.localPosition = new Vector3(0, 0, 0);
        }
        else
        {
            Debug.LogWarning("O prefab do escudo ta faltando, não tem defesa!");
        }
    }

    void Update()
    {
        if (jogadorTransform == null) return;

        Flipar();
    }

    void FixedUpdate()
    {
        if (jogadorTransform == null) return;

        float distanciaAoJogador = Vector2.Distance(transform.position, jogadorTransform.position);

        if (distanciaAoJogador <= raioDeDeteccao)
        {
            if (distanciaAoJogador > distanciaDeAtaque)
            {
                Vector2 direcao = (jogadorTransform.position - transform.position).normalized;
                rb.linearVelocity = new Vector2(direcao.x * velocidade, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        // AGORA USA A VARIÁVEL
        if (escudoInstanciado != null)
        {
            float direcaoX = (jogadorTransform.position.x - transform.position.x > 0) ? 1 : -1;
            escudoInstanciado.localPosition = new Vector3(direcaoX * distanciaEscudo, 0, 0); 
        }
    }

    // --- Colisão e Ataque ---

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (danoPrefab != null)
            {
                float direcaoInstanciar = (estaViradoParaDireita) ? 0.5f : -0.5f;
                Vector3 spawnPosition = transform.position + new Vector3(direcaoInstanciar, 0, 0);

                GameObject danoObjeto = Instantiate(danoPrefab, spawnPosition, Quaternion.identity);
                danoObjeto.tag = "Danger";

                Destroy(danoObjeto, 0.1f);
            }
        }
    }

    // --- Lógica de Flipar ---

    private void Flipar()
    {
        if (estaViradoParaDireita && jogadorTransform.position.x < transform.position.x)
        {
            timerVirar += Time.deltaTime;
            if (timerVirar >= delayParaVirar)
            {
                sp.flipX = true;
                estaViradoParaDireita = false;
                timerVirar = 0;
            }
        }
        else if (!estaViradoParaDireita && jogadorTransform.position.x > transform.position.x)
        {
            timerVirar += Time.deltaTime;
            if (timerVirar >= delayParaVirar)
            {
                sp.flipX = false;
                estaViradoParaDireita = true;
                timerVirar = 0;
            }
        }
        else
        {
            timerVirar = 0;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaDeAtaque);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, raioDeDeteccao);
    }
}