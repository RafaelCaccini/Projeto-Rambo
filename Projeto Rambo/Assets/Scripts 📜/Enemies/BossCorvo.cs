using UnityEngine;

public class BossCorvo : MonoBehaviour
{
    private LifeScript lifeScript;
    private Animator animator;

    private bool fase2Ativada = false;
    private bool executandoTransicaoFase2 = false; // trava enquanto roda a animação
    private float vidaMetade;

    public Transform pontoA;
    public Transform pontoB;
    public float velocidadeFase1 = 3f;
    public float velocidadeFase2 = 6f;
    private float velocidadeAtual;
    private Transform alvoAtual;

    public Transform firePoint;
    public GameObject projetilPrefab;
    public float detectionRange = 10f;
    public float fireCooldownFase1 = 2.5f;
    public float fireCooldownFase2 = 0.8f;
    public float velocidadeProjetilFase1 = 10f;
    public float velocidadeProjetilFase2 = 6f;
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
            player = playerObject.transform;

        if (lifeScript != null)
            vidaMetade = lifeScript.vidaMaxima / 2f;

        alvoAtual = pontoB;
        escalaOriginal = transform.localScale;
        velocidadeAtual = velocidadeFase1;
        fireCooldownAtual = fireCooldownFase1;
    }

    void Update()
    {
        if (player == null) return;

        VerificarFase();

        if (executandoTransicaoFase2) return; // trava tudo durante animação de transição

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
            executandoTransicaoFase2 = true;

            // trava movimento e desliga LifeScript
            velocidadeAtual = 0f;
            if (lifeScript != null)
                lifeScript.enabled = false;

            // dispara animação da fase 2
            animator.SetTrigger("Fase2");
        }
    }

    // chamado via Animation Event no final da animação "Fase2"
    public void FinalizarTransicaoFase2()
    {
        executandoTransicaoFase2 = false;

        // libera movimento e aumenta velocidade
        velocidadeAtual = velocidadeFase2;
        fireCooldownAtual = fireCooldownFase2;

        if (lifeScript != null)
            lifeScript.enabled = true;
    }

    void Patrulhar()
    {
        transform.position = Vector2.MoveTowards(transform.position, alvoAtual.position, velocidadeAtual * Time.deltaTime);

        if (Vector2.Distance(transform.position, alvoAtual.position) < 0.2f)
            alvoAtual = (alvoAtual == pontoA) ? pontoB : pontoA;
    }

    void VirarParaPlayer()
    {
        if (executandoTransicaoFase2) return;

        bool olhandoDireita = player.position.x > transform.position.x;

        // aqui ajustamos porque o sprite ORIGINAL olha pra ESQUERDA
        if (olhandoDireita)
        {
            // se o player está à direita → invertemos a escala
            transform.localScale = new Vector3(-Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
        }
        else
        {
            // se o player está à esquerda → mantemos a escala original
            transform.localScale = new Vector3(Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
        }
    }



    void Atacar()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireCooldownAtual)
        {
            fireTimer = 0f;
            animator.SetTrigger("Atacando"); // animação de ataque

            if (fase2Ativada)
            {
                DispararProjetil(velocidadeProjetilFase2);
                DispararProjetil(velocidadeProjetilFase2, -15f);
                DispararProjetil(velocidadeProjetilFase2, 15f);
            }
            else
            {
                DispararProjetil(velocidadeProjetilFase1);
            }
        }
    }

    void DispararProjetil(float velocidadeDeDisparo, float anguloDesvio = 0f)
    {
        if (firePoint == null || projetilPrefab == null) return;

        Vector2 direcao = (player.position - firePoint.position).normalized;
        Quaternion rotacao = Quaternion.LookRotation(Vector3.forward, direcao);

        rotacao *= Quaternion.Euler(0, 0, anguloDesvio);
        direcao = rotacao * Vector2.up;

        GameObject projetil = Instantiate(projetilPrefab, firePoint.position, rotacao);
        Rigidbody2D rb = projetil.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direcao * velocidadeDeDisparo;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (pontoA != null && pontoB != null)
            Gizmos.DrawLine(pontoA.position, pontoB.position);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
