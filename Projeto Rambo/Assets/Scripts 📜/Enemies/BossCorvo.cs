using UnityEngine;

public class BossCorvo : MonoBehaviour
{
    private LifeScript lifeScript;
    private Animator animator;

    [Header("Fases e Condies")]
    private bool fase2Ativada = false;
    private float vidaMetade;

    [Header("Patrulha e Movimento")]
    public Transform pontoA;
    public Transform pontoB;
    public float velocidadeFase1 = 3f;
    public float velocidadeFase2 = 6f; // Corvo mais rápido na Fase 2
    private float velocidadeAtual;
    private Transform alvoAtual;

    [Header("Ataque")]
    public Transform firePoint; // **Deve ser um objeto filho do Corvo na Unity!**
    public GameObject projetilPrefab;
    public float detectionRange = 10f;
    public float fireCooldownFase1 = 2.5f;
    public float fireCooldownFase2 = 0.8f; // Tiros mais frequentes
    public float velocidadeProjetilFase1 = 10f; // Velocidade do projétil normal
    public float velocidadeProjetilFase2 = 6f; // Velocidade do projétil reduzida
    private float fireCooldownAtual;

    Transform player;
    float fireTimer = 0f;

    private Vector3 escalaOriginal;

    void Start()
    {
        lifeScript = GetComponent<LifeScript>();
        animator = GetComponent<Animator>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        if (lifeScript != null)
        {
            // Nota: Corrigindo acentuação em 'Metade' para consistência
            vidaMetade = lifeScript.vidaMaxima / 2f;
        }

        alvoAtual = pontoB;
        escalaOriginal = transform.localScale;
        velocidadeAtual = velocidadeFase1;
        fireCooldownAtual = fireCooldownFase1;
    }

    void Update()
    {
        if (player == null) return;

        VerificarFase();

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            Patrulhar();
            VirarParaPlayer();
            Atacar();
        }
        else
        {
            Patrulhar();
        }
    }

    void VerificarFase()
    {
        if (!fase2Ativada && lifeScript != null && lifeScript.vidaAtual <= vidaMetade)
        {
            fase2Ativada = true;
            velocidadeAtual = velocidadeFase2;
            fireCooldownAtual = fireCooldownFase2;
            Debug.Log("Boss Corvo entrou na FASE 2: Mais rápido, mais disparos, projéteis lentos!");

            if (animator != null)
            {
                // animator.SetTrigger("Furia");
            }
        }
    }

    void Patrulhar()
    {
        transform.position = Vector2.MoveTowards(transform.position, alvoAtual.position, velocidadeAtual * Time.deltaTime);

        if (Vector2.Distance(transform.position, alvoAtual.position) < 0.2f)
        {
            alvoAtual = (alvoAtual == pontoA) ? pontoB : pontoA;
        }
        // Nota: A virada de Patrulha foi removida, mantendo VirarParaPlayer() responsável pela orientação
    }

    void VirarParaPlayer()
    {
        Vector3 escala = escalaOriginal;
        bool olhandoDireita = player.position.x > transform.position.x;
        escala.x = olhandoDireita ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);
        transform.localScale = escala;
    }

    void Atacar()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireCooldownAtual)
        {
            fireTimer = 0f;

            if (fase2Ativada)
            {
                // FASE 2: Três disparos com velocidadeProjetilFase2
                DispararProjetil(velocidadeProjetilFase2);
                DispararProjetil(velocidadeProjetilFase2, -15f);
                DispararProjetil(velocidadeProjetilFase2, 15f);
            }
            else
            {
                // FASE 1: Disparo único com velocidadeProjetilFase1
                DispararProjetil(velocidadeProjetilFase1);
            }

            if (animator != null)
            {
                // animator.SetTrigger("Atirar");
            }
        }
    }

    // MÉTODO AJUSTADO para receber a velocidade correta
    void DispararProjetil(float velocidadeDeDisparo, float anguloDesvio = 0f)
    {
        if (firePoint == null || projetilPrefab == null) return;

        // 1. Calcula a direção e rotação do tiro
        Vector2 direcao = (player.position - firePoint.position).normalized;
        Quaternion rotacao = Quaternion.LookRotation(Vector3.forward, direcao);

        // Aplica o desvio angular 
        rotacao *= Quaternion.Euler(0, 0, anguloDesvio);
        direcao = rotacao * Vector2.up;

        // 2. Instancia o projétil e aplica a velocidade
        GameObject projetil = Instantiate(projetilPrefab, firePoint.position, rotacao);

        Rigidbody2D rb = projetil.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Aplica a velocidade específica da fase
            rb.linearVelocity = direcao * velocidadeDeDisparo;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (pontoA != null && pontoB != null)
        {
            Gizmos.DrawLine(pontoA.position, pontoB.position);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}