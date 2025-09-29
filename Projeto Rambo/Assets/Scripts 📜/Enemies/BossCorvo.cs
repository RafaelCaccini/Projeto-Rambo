using UnityEngine;

public class BossCorvo : MonoBehaviour
{
    private LifeScript lifeScript;   // controla a vida do chefe
    private Animator animator;       // controla as animações dele

    // controla quando o chefe muda para a segunda fase
    private bool fase2Ativada = false;
    private float vidaMetade;

    // pontos que o chefe vai andar de um lado pro outro
    public Transform pontoA;
    public Transform pontoB;
    public float velocidadeFase1 = 3f;
    public float velocidadeFase2 = 6f; // mais rápido na fase 2
    private float velocidadeAtual;
    private Transform alvoAtual;

    // configurações de ataque do chefe
    public Transform firePoint;           // lugar onde o tiro nasce
    public GameObject projetilPrefab;     // o tiro em si
    public float detectionRange = 10f;    // distância que ele "vê" o jogador
    public float fireCooldownFase1 = 2.5f;
    public float fireCooldownFase2 = 0.8f; // atira mais rápido na fase 2
    public float velocidadeProjetilFase1 = 10f;
    public float velocidadeProjetilFase2 = 6f;
    private float fireCooldownAtual;

    Transform player;    // referência pro jogador
    float fireTimer = 0f;

    private Vector3 escalaOriginal;  // guarda a escala original pra virar o chefe

    void Start()
    {
        // pega os componentes de vida e animação do chefe
        lifeScript = GetComponent<LifeScript>();
        animator = GetComponent<Animator>();

        // procura o jogador na cena
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;

        // calcula qual é a metade da vida total
        if (lifeScript != null)
            vidaMetade = lifeScript.vidaMaxima / 2f;

        // define ponto inicial da patrulha e velocidade inicial
        alvoAtual = pontoB;
        escalaOriginal = transform.localScale;
        velocidadeAtual = velocidadeFase1;
        fireCooldownAtual = fireCooldownFase1;
    }

    void Update()
    {
        if (player == null) return;

        VerificarFase(); // checa se precisa mudar pra fase 2

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // se o jogador estiver perto, ele persegue e ataca
        if (distanceToPlayer <= detectionRange)
        {
            Patrulhar();
            VirarParaPlayer();
            Atacar();
        }
        else
        {
            // se não estiver perto, só patrulha
            Patrulhar();
        }
    }

    void VerificarFase()
    {
        // muda pra fase 2 quando a vida estiver pela metade
        if (!fase2Ativada && lifeScript != null && lifeScript.vidaAtual <= vidaMetade)
        {
            fase2Ativada = true;
            velocidadeAtual = velocidadeFase2;
            fireCooldownAtual = fireCooldownFase2;
            Debug.Log("Boss Corvo entrou na FASE 2: Mais rápido, mais disparos, projéteis lentos!");
        }
    }

    void Patrulhar()
    {
        // movimenta o chefe entre os pontos A e B
        transform.position = Vector2.MoveTowards(transform.position, alvoAtual.position, velocidadeAtual * Time.deltaTime);

        // troca o alvo quando chega perto
        if (Vector2.Distance(transform.position, alvoAtual.position) < 0.2f)
            alvoAtual = (alvoAtual == pontoA) ? pontoB : pontoA;
    }

    void VirarParaPlayer()
    {
        // faz o chefe "virar" pro lado do jogador
        Vector3 escala = escalaOriginal;
        bool olhandoDireita = player.position.x > transform.position.x;
        escala.x = olhandoDireita ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);
        transform.localScale = escala;
    }

    void Atacar()
    {
        fireTimer += Time.deltaTime;

        // dispara quando o tempo de recarga acabar
        if (fireTimer >= fireCooldownAtual)
        {
            fireTimer = 0f;

            if (fase2Ativada)
            {
                // na fase 2 atira 3 projéteis com ângulos diferentes
                DispararProjetil(velocidadeProjetilFase2);
                DispararProjetil(velocidadeProjetilFase2, -15f);
                DispararProjetil(velocidadeProjetilFase2, 15f);
            }
            else
            {
                // na fase 1 atira só 1 projétil
                DispararProjetil(velocidadeProjetilFase1);
            }
        }
    }

    void DispararProjetil(float velocidadeDeDisparo, float anguloDesvio = 0f)
    {
        if (firePoint == null || projetilPrefab == null) return;

        // calcula a direção do tiro em direção ao jogador
        Vector2 direcao = (player.position - firePoint.position).normalized;
        Quaternion rotacao = Quaternion.LookRotation(Vector3.forward, direcao);

        // aplica o ângulo de desvio se tiver
        rotacao *= Quaternion.Euler(0, 0, anguloDesvio);
        direcao = rotacao * Vector2.up;

        // cria o projétil e aplica a velocidade
        GameObject projetil = Instantiate(projetilPrefab, firePoint.position, rotacao);
        Rigidbody2D rb = projetil.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direcao * velocidadeDeDisparo;
    }

    void OnDrawGizmosSelected()
    {
        // desenha no editor o caminho da patrulha e o alcance de detecção
        Gizmos.color = Color.yellow;
        if (pontoA != null && pontoB != null)
            Gizmos.DrawLine(pontoA.position, pontoB.position);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
