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
    public LayerMask playerLayer;

    [Header("Espinhos")]
    public GameObject spikePrefab;
    public Transform[] spikeSpawnPoints;
    public float spikeSpawnDelay = 2f;

    [Header("Ativação")]
    public Transform player;
    public float activationRadius = 5f;
    private bool isActivated = false;

    [Header("Áudio")]
    public AudioSource dashAudio;   // som do dash
    public AudioSource spikeAudio;  // som dos espinhos

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
            yield return new WaitForSeconds(chargeTime);

            // DASH
            yield return StartCoroutine(Dash());

            // ESPINHOS
            yield return new WaitForSeconds(spikeSpawnDelay);
            SpawnSpikes();
        }
    }

    private IEnumerator Dash()
    {
        Vector3 target = movingToB ? pointB.position : pointA.position;

        // toca som do dash
        if (dashAudio != null)
            dashAudio.Play();

        if (bossCollider != null)
            bossCollider.enabled = false;

        bool hasDamagedPlayer = false;

        while (Vector2.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, dashSpeed * Time.deltaTime);

            // checa hit no player
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, dashHitRadius, playerLayer);
            foreach (var hit in hits)
            {
                LifeScript life = hit.GetComponent<LifeScript>();
                if (life != null)
                {
                    life.TomarDano(damage);
                    hasDamagedPlayer = true;
                    break;
                }
            }

            yield return null;
        }

        transform.position = target;
        movingToB = !movingToB;

        if (bossCollider != null)
            bossCollider.enabled = true;
    }

    private void SpawnSpikes()
    {
        // toca som dos espinhos
        if (spikeAudio != null)
            spikeAudio.Play();

        if (spikeSpawnPoints.Length == 0 || spikePrefab == null) return;

        foreach (Transform spawnPoint in spikeSpawnPoints)
        {
            GameObject spike = Instantiate(spikePrefab, spawnPoint.position, Quaternion.identity);

            Collider2D spikeCol = spike.GetComponent<Collider2D>();
            if (spikeCol != null && bossCollider != null)
                Physics2D.IgnoreCollision(spikeCol, bossCollider);

            Spike spikeScript = spike.GetComponent<Spike>();
            if (spikeScript == null)
            {
                spikeScript = spike.AddComponent<Spike>();
                spikeScript.damage = 15;
            }
        }
    }

    public void TomarDano(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
            Destroy(gameObject);
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
