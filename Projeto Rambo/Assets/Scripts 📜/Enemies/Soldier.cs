using UnityEngine;

public class Soldier : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 2f;
    public Transform pontoA;
    public Transform pontoB;
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

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        alvoAtual = pontoB;
        escalaOriginal = transform.localScale;
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        playerInRange = distanceToPlayer <= detectionRange;

        if (playerInRange)
        {
            VirarParaPlayer();
            Atacar();
        }
        else
        {
            Patrulhar();
        }
    }

    void Patrulhar()
    {
        transform.position = Vector2.MoveTowards(transform.position, alvoAtual.position, velocidade * Time.deltaTime);

        if (Vector2.Distance(transform.position, alvoAtual.position) < 0.2f)
        {
            alvoAtual = (alvoAtual == pontoA) ? pontoB : pontoA;
        }

        FlipVisual(alvoAtual.position.x > transform.position.x);
    }

    void VirarParaPlayer()
    {
        FlipVisual(player.position.x > transform.position.x);
    }

    void FlipVisual(bool olhandoDireita)
    {
        // Flipa inimigo E o firePoint junto
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

            if (player.position.y > transform.position.y + 1f)
            {
                // Disparo pra cima
                GameObject bulletUp = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
                bulletUp.transform.rotation = Quaternion.Euler(0, 0, 90);
                bulletUp.GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * bulletSpeed;
            }
            else
            {
                // Disparo reto (lado que o inimigo está olhando)
                Vector2 dir = (transform.localScale.x > 0) ? Vector2.right : Vector2.left;

                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
                bullet.GetComponent<Rigidbody2D>().linearVelocity = dir * bulletSpeed;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        if (pontoA != null && pontoB != null)
        {
            Gizmos.DrawLine(pontoA.position, pontoB.position);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
