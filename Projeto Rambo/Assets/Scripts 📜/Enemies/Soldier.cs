using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Soldier : MonoBehaviour
{
    // A referência ao Animator para as animações
    private Animator animator;

    [Header("Movimento")]
    public float velocidade = 2f;
    public Transform pontoA;
    public Transform pontoB;
    [Tooltip("Marque para usar o ponto C como destino inicial.")]
    public bool usarPontoC = false;
    public Transform pontoC;
    private Transform alvoAtual;

    [Header("Ataque")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float fireCooldown = 2f;
    public float detectionRange = 6f;
    public float bulletSpeed = 8f;

    Transform player;
    float fireTimer = 0f;
    bool playerInRange = false;

    private Vector3 escalaOriginal;
    private bool completouPontoC;

    void Start()
    {
        animator = GetComponent<Animator>(); // Obtém a referência ao Animator

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        // Lógica inicial para definir o primeiro destino
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

        // Inicia a animação de andar
        if (animator != null)
        {
            animator.SetBool("Andando", true);
        }
    }

    void Update()
    {
        // Se o player não for encontrado, apenas patrulha
        if (player == null)
        {
            Patrulhar();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        playerInRange = distanceToPlayer <= detectionRange;

        if (playerInRange)
        {
            VirarParaPlayer();
            Atacar();
            // Para a animação de andar
            if (animator != null)
            {
                animator.SetBool("Andando", false);
            }
        }
        else
        {
            Patrulhar();
            // Retoma a animação de andar
            if (animator != null)
            {
                animator.SetBool("Andando", true);
            }
        }
    }

    void Patrulhar()
    {
        // Move o soldado em direção ao próximo ponto
        transform.position = Vector2.MoveTowards(transform.position, alvoAtual.position, velocidade * Time.deltaTime);

        if (Vector2.Distance(transform.position, alvoAtual.position) < 0.2f)
        {
            // Chegou ao destino, agora decide o próximo passo
            if (!completouPontoC)
            {
                // Se completou o ponto C, agora começa a patrulhar A e B
                completouPontoC = true;
                alvoAtual = pontoB;
            }
            else
            {
                // Alterna entre os pontos A e B
                alvoAtual = (alvoAtual == pontoA) ? pontoB : pontoA;
            }
        }

        // Vira para o próximo ponto de patrulha
        FlipVisual(alvoAtual.position.x > transform.position.x);
    }

    void VirarParaPlayer()
    {
        FlipVisual(player.position.x > transform.position.x);
    }

    void FlipVisual(bool olhandoDireita)
    {
        Vector3 escala = escalaOriginal;
        escala.x = olhandoDireita ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);
        transform.localScale = escala;
    }

    void Atacar()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireCooldown)
        {
            fireTimer = 0f;

            // Comentário para a animação de ataque/tiro:
            // if (animator != null) { animator.SetTrigger("Atirar"); }

            // Lógica para atirar APENAS na horizontal
            Vector2 dir;
            if (transform.localScale.x > 0)
            {
                // Inimigo virado para a direita
                dir = Vector2.right;
            }
            else
            {
                // Inimigo virado para a esquerda
                dir = Vector2.left;
            }

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().linearVelocity = dir * bulletSpeed;
        }
    }

    void OnDrawGizmosSelected()
    {
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