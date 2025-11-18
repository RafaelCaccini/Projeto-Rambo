using UnityEngine;
using UnityEngine.SceneManagement;

public class BossCorvo : MonoBehaviour
{
    private LifeScript lifeScript;
    private Animator animator;

    private bool fase2Ativada = false;
    private bool executandoTransicaoFase2 = false;
    private bool jaGritouAoDetectar = false;
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

    private Transform player;
    private float fireTimer = 0f;
    private Vector3 escalaOriginal;

    [Header("Sons do Corvo")]
    public AudioSource gritoSource; // arraste o som de grito
    public AudioSource pocaoSource; // arraste o som da poção caindo

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

        if (executandoTransicaoFase2) return;

        float distancia = Vector2.Distance(transform.position, player.position);

        // Grito ao detectar o player
        if (distancia <= detectionRange && !jaGritouAoDetectar)
        {
            jaGritouAoDetectar = true;
            if (gritoSource != null) ;
               
        }

        if (distancia <= detectionRange)
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

            velocidadeAtual = 0f;
            if (lifeScript != null)
                lifeScript.enabled = false;

            animator.SetTrigger("Fase2");
        }
    }

    public void FinalizarTransicaoFase2()
    {
        executandoTransicaoFase2 = false;
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

        if (olhandoDireita)
            transform.localScale = new Vector3(-Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
        else
            transform.localScale = new Vector3(Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
    }

    void Atacar()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireCooldownAtual)
        {
            fireTimer = 0f;
            animator.SetTrigger("Atacando");

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

            // Som da poção sendo jogada
            if (pocaoSource != null)
                pocaoSource.Play();
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

    public void TrocarCena(string nomeCena)
    {
        if (!string.IsNullOrEmpty(nomeCena))
            SceneManager.LoadScene(nomeCena);
        else
            Debug.LogWarning("Nome da cena não definido para troca!");
    }

    public void Grito()
    {
            gritoSource.Play();
    }
}
