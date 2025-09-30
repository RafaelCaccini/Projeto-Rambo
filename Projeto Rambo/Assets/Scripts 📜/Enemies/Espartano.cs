using UnityEngine;

public class Espartano : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [Tooltip("Velocidade do Espartano ao se mover.")]
    public float velocidade = 2.5f;
    [Tooltip("Distância para o Espartano começar a andar em direção ao jogador.")]
    public float raioDeDeteccao = 5f;
    private Rigidbody2D rb;

    [Header("Configurações de Virar")]
    [Tooltip("Tempo de atraso para o Espartano virar após o jogador passar para o outro lado.")]
    public float delayParaVirar = 0.5f;
    private bool estaViradoParaDireita = true;
    public bool EstaViradoParaDireita { get { return estaViradoParaDireita; } }
    private float timerVirar;
    [Tooltip("Arraste o objeto 'Visuals' filho do Espartano aqui.")]
    public Transform visualsTransform;

    [Header("Ataque e Defesa")]
    [Tooltip("Distância que o Espartano precisa estar do jogador para atacar.")]
    public float distanciaDeAtaque = 0.8f;
    [Tooltip("Prefab do objeto de escudo que será gerado.")]
    public GameObject escudoPrefab;
    [Tooltip("Distância que o escudo aparece à frente do Espartano.")]
    public float distanciaEscudo = 0.5f;
    private Transform escudoInstanciado;

    [Header("Referências")]
    [Tooltip("Arraste e solte o objeto do jogador aqui.")]
    public Transform jogadorTransform;

    //---------------------------------------------------------

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (escudoPrefab != null)
        {
            // O escudo agora é filho do objeto 'Visuals'
            escudoInstanciado = Instantiate(escudoPrefab, visualsTransform).transform;
            escudoInstanciado.localPosition = new Vector3(0, 0, 0);
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

        if (escudoInstanciado != null)
        {
            float direcaoX = (jogadorTransform.position.x - transform.position.x > 0) ? 1 : -1;
            escudoInstanciado.localPosition = new Vector3(direcaoX * distanciaEscudo, 0, 0);
        }
    }

    public void TomarDano(float dano)
    {
        Debug.Log("Espartano tomou dano!");
    }

    // --- Lógica de Flipar ---

    private void Flipar()
    {
        if (estaViradoParaDireita && jogadorTransform.position.x < transform.position.x)
        {
            timerVirar += Time.deltaTime;
            if (timerVirar >= delayParaVirar)
            {
                // Agora vira a escala do objeto 'Visuals'
                visualsTransform.localScale = new Vector3(-1, 1, 1);
                estaViradoParaDireita = false;
                timerVirar = 0;
            }
        }
        else if (!estaViradoParaDireita && jogadorTransform.position.x > transform.position.x)
        {
            timerVirar += Time.deltaTime;
            if (timerVirar >= delayParaVirar)
            {
                // Vira a escala do objeto 'Visuals'
                visualsTransform.localScale = new Vector3(1, 1, 1);
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