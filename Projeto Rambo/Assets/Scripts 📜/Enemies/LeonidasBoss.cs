using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 200;
    private int currentHealth;

    [Header("Movimento")]
    public Transform pointA;
    public Transform pointB;
    public float dashSpeed = 25f;

    [Header("Ataque")]
    public float chargeTime = 3f;
    public int damage = 20;
    public float dashHitRadius = 1.5f;
    public LayerMask playerLayer; // Layer do Player

    [Header("Espinhos")]
    public GameObject spikePrefab;
    public Transform[] spikeSpawnPoints;
    public float spikeSpawnDelay = 2f;

    [Header("Ativação")]
    public Transform player;
    public float activationRadius = 5f;
    private bool isActivated = false;

    private bool movingToB = true;
    private Collider2D bossCollider;

    void Start()
    {
        currentHealth = maxHealth;
        bossCollider = GetComponent<Collider2D>();
        gameObject.tag = "Boss";
    }

    void Update()
    {
        if (!isActivated && player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance <= activationRadius)
            {
                isActivated = true;
                StartCoroutine(AttackCycle());
            }
        }
    }

    private IEnumerator AttackCycle()
    {
        while (currentHealth > 0)
        {
            // 1. Carrega ataque
            yield return new WaitForSeconds(chargeTime);

            // 2. Executa dash
            yield return StartCoroutine(Dash());

            // 3. Spawn espinhos após dash
            yield return new WaitForSeconds(spikeSpawnDelay);
            SpawnSpikes();
        }
    }

    private IEnumerator Dash()
    {
        Vector3 target = movingToB ? pointB.position : pointA.position;

        // DESATIVA collider do boss para atravessar o player
        if (bossCollider != null) bossCollider.enabled = false;

        bool hasDamagedPlayer = false;

        while (Vector2.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, dashSpeed * Time.deltaTime);

            // cria hitbox temporária para dano
            if (!hasDamagedPlayer)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, dashHitRadius, playerLayer);
                foreach (var hit in hits)
                {
                    LifeScript life = hit.GetComponent<LifeScript>();
                    if (life != null)
                    {
                        life.TomarDano(damage);
                        hasDamagedPlayer = true; // só uma vez
                        break;
                    }
                }
            }

            yield return null;
        }

        // garante que ele esteja exatamente no ponto
        transform.position = target;

        // Alterna ponto
        movingToB = !movingToB;

        // REATIVA collider
        if (bossCollider != null) bossCollider.enabled = true;
    }

    private void SpawnSpikes()
    {
        if (spikeSpawnPoints.Length == 0 || spikePrefab == null) return;

        foreach (Transform spawnPoint in spikeSpawnPoints)
        {
            GameObject spike = Instantiate(spikePrefab, spawnPoint.position, Quaternion.identity);

            // garante que o spike não colida com o boss
            Collider2D spikeCol = spike.GetComponent<Collider2D>();
            if (spikeCol != null && bossCollider != null)
                Physics2D.IgnoreCollision(spikeCol, bossCollider);

            // garante que o spike tenha o script de dano ativo
            Spike spikeScript = spike.GetComponent<Spike>();
            if (spikeScript == null)
            {
                spikeScript = spike.AddComponent<Spike>();
                spikeScript.damage = 15; // ou qualquer valor que você quiser
            }
        }
    }


    public void TomarDano(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, activationRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, dashHitRadius);

        if (spikeSpawnPoints != null)
        {
            Gizmos.color = Color.green;
            foreach (Transform t in spikeSpawnPoints)
            {
                if (t != null)
                    Gizmos.DrawWireSphere(t.position, 0.2f);
            }
        }
    }
}
