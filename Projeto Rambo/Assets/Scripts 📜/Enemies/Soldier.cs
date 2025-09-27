using UnityEngine;

public class Soldier : MonoBehaviour
{
    private Animator animator; // controla as animações do soldado

    [Header("Movimento")]
    public float velocidade = 2f; // velocidade do soldado
    public Transform pontoA; // ponto inicial da patrulha
    public Transform pontoB; // ponto final da patrulha
    public bool usarPontoC = false; // se vai começar indo pro ponto C
    public Transform pontoC; // ponto opcional de início
    private Transform alvoAtual; // ponto atual que ele vai

    [Header("Ataque")]
    public Transform firePoint; // lugar de onde a bala sai
    public GameObject bulletPrefab; // bala que ele dispara
    public float fireCooldown = 2f; // tempo entre tiros
    public float detectionRange = 6f; // distância pra ele ver o jogador
    public float bulletSpeed = 8f; // velocidade da bala

    Transform player; // referência ao jogador
    float fireTimer = 0f; // cronômetro pro tiro
    bool playerInRange = false; // se o jogador tá perto

    private Vector3 escalaOriginal; // guarda a escala inicial
    private bool completouPontoC; // se ele já passou pelo ponto C

    void Start()
    {
        animator = GetComponent<Animator>(); // pega o animador

        // acha o jogador na cena
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        // define pra onde ir primeiro
        if (usarPontoC && pontoC != null)
        {
            alvoAtual = pontoC;
            completouPontoC = false;
        }
        else
        {
            alvoAtual = pontoB;
            completouPontoC = true;
        }

        escalaOriginal = transform.localScale;

        // começa com a animação de andar
        if (animator != null)
        {
            animator.SetBool("Andando", true);
        }
    }

    void Update()
    {
        // se não tiver jogador, ele só patrulha
        if (player == null)
        {
            Patrulhar();
            return;
        }

        // vê se o jogador está perto
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        playerInRange = distanceToPlayer <= detectionRange;

        if (playerInRange)
        {
            VirarParaPlayer(); // vira pro jogador
            Atacar(); // começa a atirar
            if (animator != null)
            {
                animator.SetBool("Andando", false); // para de andar
            }
        }
        else
        {
            Patrulhar(); // continua andando
            if (animator != null)
            {
                animator.SetBool("Andando", true);
            }
        }
    }

    void Patrulhar()
    {
        // anda até o ponto atual
        transform.position = Vector2.MoveTowards(transform.position, alvoAtual.position, velocidade * Time.deltaTime);

        // quando chega no ponto, decide o próximo
        if (Vector2.Distance(transform.position, alvoAtual.position) < 0.2f)
        {
            if (!completouPontoC)
            {
                completouPontoC = true;
                alvoAtual = pontoB;
            }
            else
            {
                alvoAtual = (alvoAtual == pontoA) ? pontoB : pontoA;
            }
        }

        // vira na direção que está indo
        FlipVisual(alvoAtual.position.x > transform.position.x);
    }

    void VirarParaPlayer()
    {
        // vira pro lado do jogador
        FlipVisual(player.position.x > transform.position.x);
    }

    void FlipVisual(bool olhandoDireita)
    {
        // espelha o sprite pra esquerda ou direita
        Vector3 escala = escalaOriginal;
        escala.x = olhandoDireita ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);
        transform.localScale = escala;
    }

    void Atacar()
    {
        fireTimer += Time.deltaTime;

        // atira se passou o tempo do cooldown
        if (fireTimer >= fireCooldown)
        {
            fireTimer = 0f;

            // define a direção do tiro baseado no lado que ele está virado
            Vector2 dir = (transform.localScale.x > 0) ? Vector2.right : Vector2.left;

            // cria a bala e aplica velocidade
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().linearVelocity = dir * bulletSpeed;

            // toca a animação de tiro
            animator.SetTrigger("Atirar");
        }
    }

    void OnDrawGizmosSelected()
    {
        // mostra as linhas dos pontos de patrulha e o alcance de visão
        Gizmos.color = Color.green;
        if (pontoA != null && pontoB != null)
        {
            Gizmos.DrawLine(pontoA.position, pontoB.position);
        }
        if (usarPontoC && pontoC != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, pontoC.position);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}

// Mariane Duarte Nunes
// 2025
