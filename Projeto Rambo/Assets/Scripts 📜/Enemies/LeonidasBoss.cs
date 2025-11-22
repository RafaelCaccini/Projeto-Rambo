using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [Tooltip("Tempo de preparação do ataque (pose antes do dash)")]
    public float chargeTime = 1.2f; // reduzido — ataques mais rápidos
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
    public AudioSource dashAudio;
    public AudioSource spikeAudio;
    public AudioSource musica;

    private bool movingToB = false; // começa indo de B → A
    private Collider2D bossCollider;
    private Animator anim;
    private bool movimentoTravado = false;

    void Start()
    {
        currentHealth = maxHealth;
        bossCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        gameObject.tag = "Boss";

        // começa virado para o ponto A
        if (pointA != null)
            FlipTowards(pointA.position);
    }

    void Update()
    {
        if (movimentoTravado) return;

        if (!isActivated && player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance <= activationRadius)
            {
                isActivated = true;
                StartCoroutine(AttackCycle());
                musica.Play();
            }
        }
    }

    private IEnumerator AttackCycle()
    {
        while (currentHealth > 0)
        {
            // 1️⃣ Bater o pé (invocar lanças)
            anim.SetTrigger("BaterPe");
            yield return new WaitForSeconds(1f);

            SpawnSpikes();
            yield return new WaitForSeconds(spikeSpawnDelay);

            // 2️⃣ Pose de ataque (vira e prepara)
            Vector3 nextTarget = movingToB ? pointB.position : pointA.position;
            FlipTowards(nextTarget);
            anim.SetTrigger("PoseAtaque");
            yield return new WaitForSeconds(chargeTime);

            // 3️⃣ Dash (ataque)
            yield return StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        if (movimentoTravado) yield break;

        Vector3 target = movingToB ? pointB.position : pointA.position;
        anim.SetBool("Atacando", true);

        dashAudio?.Play();
        if (bossCollider != null)
            bossCollider.enabled = false;

        bool hasDamagedPlayer = false;

        while (Vector2.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, dashSpeed * Time.deltaTime);

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, dashHitRadius, playerLayer);
            foreach (var hit in hits)
            {
                LifeScript life = hit.GetComponent<LifeScript>();
                if (life != null && !hasDamagedPlayer)
                {
                    life.TomarDano(damage);
                    hasDamagedPlayer = true;
                }
            }

            yield return null;
        }

        transform.position = target;
        anim.SetBool("Atacando", false);
        movingToB = !movingToB; // alterna para o próximo alvo

        if (bossCollider != null)
            bossCollider.enabled = true;

        if (player != null)
            FlipTowards(player.position);
    }

    private void SpawnSpikes()
    {
        spikeAudio?.Play();

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

    private void FlipTowards(Vector3 target)
    {
        Vector3 scale = transform.localScale;

        // 🔁 Lógica invertida de flip
        if (target.x > transform.position.x && scale.x > 0)
            scale.x *= -1;
        else if (target.x < transform.position.x && scale.x < 0)
            scale.x *= -1;

        transform.localScale = scale;
    }

    public void TomarDano(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            anim.SetTrigger("Morrendo");
            Destroy(gameObject, 2f);
        }
    }

    // 🔒 Travar/destravar movimento — chamado por Animation Event
    public void TravarMovimento(bool travar)
    {
        movimentoTravado = travar;
    }

    // 🔄 Trocar cena — chamado por Animation Event
    public void TrocarCena(string nomeCena)
    {
        SceneManager.LoadScene(nomeCena);
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
