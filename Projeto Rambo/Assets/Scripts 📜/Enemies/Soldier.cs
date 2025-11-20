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

        // quando morrer
        lifeScript.OnMorte += OnMorte;
    }

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;

        // define o ponto inicial da patrulha
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

        if (animator)
            animator.SetBool("Andando", true);
    }

    void Update()
    {
        if (lifeScript.GetVidaAtual() <= 0)
            return;

        if (player == null)
        {
            Patrulhar();
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        playerInRange = distance <= detectionRange;

        if (playerInRange)
        {
            VirarPara(player.position.x);
            Atacar();
            if (animator) animator.SetBool("Andando", false);
        }
        else
        {
            Patrulhar();
            if (animator) animator.SetBool("Andando", true);
        }
    }

    void Patrulhar()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            alvoAtual.position,
            velocidade * Time.deltaTime
        );

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

        VirarPara(alvoAtual.position.x);
    }

    void VirarPara(float posX)
    {
        Vector3 escala = escalaOriginal;
        escala.x = (posX > transform.position.x) ? Mathf.Abs(escalaOriginal.x) : -Mathf.Abs(escalaOriginal.x);
        transform.localScale = escala;
    }

    void Atacar()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer < fireCooldown) return;

        fireTimer = 0f;

        if (firePoint == null || bulletPrefab == null)
        {
            Debug.LogWarning("Soldier sem FirePoint ou BulletPrefab!");
            return;
        }

        Vector2 dir = (transform.localScale.x > 0) ? Vector2.right : Vector2.left;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("A bulletPrefab não tem Rigidbody2D!");
            Destroy(bullet);
            return;
        }

        rb.linearVelocity = dir * bulletSpeed;

        if (animator)
            animator.SetTrigger("Atirar");
    }

    void OnMorte()
    {
        if (animator)
            animator.SetTrigger("InimigoMorrendo");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        if (pontoA && pontoB)
            Gizmos.DrawLine(pontoA.position, pontoB.position);

        if (usarPontoC && pontoC)
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
