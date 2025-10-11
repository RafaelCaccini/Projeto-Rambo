using UnityEngine;

[RequireComponent(typeof(LifeScript))]
public class Soldier : MonoBehaviour
{
    private Animator animator;
    private LifeScript lifeScript;

    [Header("Movimento")]
    public float velocidade = 2f;
    public Transform pontoA;
    public Transform pontoB;
    public bool usarPontoC = false;
    public Transform pontoC;
    private Transform alvoAtual;
    private Vector3 escalaOriginal;
    private bool completouPontoC;

    [Header("Ataque")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float fireCooldown = 2f;
    public float detectionRange = 6f;
    public float bulletSpeed = 8f;

    private Transform player;
    private float fireTimer = 0f;
    private bool playerInRange = false;

    void Awake()
    {
        lifeScript = GetComponent<LifeScript>();
        animator = GetComponent<Animator>();

        // conecta LifeScript com Soldier (opcional: pode reagir a morte)
        lifeScript.OnMorte += OnMorte;
    }

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) player = playerObject.transform;

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
        if (animator != null) animator.SetBool("Andando", true);
    }

    void Update()
    {
        if (lifeScript.GetVidaAtual() <= 0) return; // bloqueia updates após morte

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
            if (animator != null) animator.SetBool("Andando", false);
        }
        else
        {
            Patrulhar();
            if (animator != null) animator.SetBool("Andando", true);
        }
    }

    void Patrulhar()
    {
        transform.position = Vector2.MoveTowards(transform.position, alvoAtual.position, velocidade * Time.deltaTime);

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
            Vector2 dir = (transform.localScale.x > 0) ? Vector2.right : Vector2.left;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().linearVelocity = dir * bulletSpeed;
            if (animator != null) animator.SetTrigger("Atirar");
        }
    }

    // este método é chamado quando o LifeScript detecta que morreu
    void OnMorte()
    {
        // dispara a animação de morte
        if (animator != null)
            animator.SetTrigger("InimigoMorrendo");

        // aqui não destrói nem troca de cena: Animation Event na animação cuida disso
        // se quiser, pode bloquear movimento e ataque
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        if (pontoA != null && pontoB != null)
            Gizmos.DrawLine(pontoA.position, pontoB.position);

        if (usarPontoC && pontoC != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, pontoC.position);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    void DestruirPosMorte()
    {
        Destroy(gameObject);
    }
}
