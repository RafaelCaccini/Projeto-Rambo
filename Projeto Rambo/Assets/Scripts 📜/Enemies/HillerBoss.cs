using UnityEngine;
using System.Collections.Generic;

public class ChefeHiller : MonoBehaviour
{
    [Header("Referências")]
    public Transform jogador;

    [Header("Spawn / Projétil")]
    public Transform PontoTiroHiller;
    public GameObject prefabProjeteil;

    [Header("Ajustes de Projétil")]
    public Vector2 localSpawnOffset = new Vector2(0.6f, 0.2f);
    public float offsetForward = 0.2f;
    public float velocidadeTiro = 10f;
    public float cooldownTiro = 2f;

    [Header("Spawn / Soldados")]
    public Transform[] pontosSpawnSoldado;
    public GameObject prefabSoldado;
    public float cooldownSpawnSoldado = 5f;
    public float forcaSpawnVertical = 5f;

    [Header("Chão do Hiller")]
    public GameObject chaoDoHillerRoot;

    [Header("Ativação")]
    public float raioAtivacao = 5f;

    private float timerTiro = 0f;
    private float timerSpawnSoldado = 0f;
    private bool ativo = false;
    private Vector3 initialScale;

    // Armazena todos os colliders do próprio Hiller
    private Collider2D[] hillerColliders;

    void Start()
    {
        initialScale = transform.localScale;
        initialScale.z = 1f;

        timerTiro = 0f;
        timerSpawnSoldado = 0f;

        // **NOVO:** Captura todos os colliders do Hiller (e seus filhos) apenas uma vez
        hillerColliders = GetComponentsInChildren<Collider2D>();
    }

    void Update()
    {
        if (jogador == null) return;

        UpdateFacing();

        if (!ativo && Vector2.Distance(transform.position, jogador.position) <= raioAtivacao)
            ativo = true;

        if (!ativo) return;

        timerTiro -= Time.deltaTime;
        if (timerTiro <= 0f)
        {
            AtirarBazuca();
            timerTiro = cooldownTiro;
        }

        timerSpawnSoldado -= Time.deltaTime;
        if (timerSpawnSoldado <= 0f)
        {
            SpawnSoldados();
            timerSpawnSoldado = cooldownSpawnSoldado;
        }
    }

    // --- Flip do Hiller ---
    void UpdateFacing()
    {
        bool facingRight = jogador.position.x > transform.position.x;
        Vector3 s = initialScale;
        s.x = Mathf.Abs(initialScale.x) * (facingRight ? -1f : 1f);
        transform.localScale = s;
    }

    // --- Tiro ---
    void AtirarBazuca()
    {
        if (prefabProjeteil == null || jogador == null || PontoTiroHiller == null)
        {
            Debug.LogError("Configuração de projétil incompleta.");
            return;
        }

        Vector2 direcao = (jogador.position - PontoTiroHiller.position).normalized;
        Vector3 spawnPos = PontoTiroHiller.position + (Vector3)direcao * offsetForward;
        float angle = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0f, 0f, angle);

        GameObject proj = Instantiate(prefabProjeteil, spawnPos, rot);

        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direcao * velocidadeTiro;
        }

        // **NOVO:** Ignorar colisão com o próprio Hiller
        IgnoreCollisionWithObject(proj, hillerColliders);

        // Ignorar colisão com o chão do boss
        if (chaoDoHillerRoot != null)
        {
            Collider2D[] chaoColliders = chaoDoHillerRoot.GetComponentsInChildren<Collider2D>();
            IgnoreCollisionWithObject(proj, chaoColliders);
        }
    }

    // --- Spawn de Soldados ---
    void SpawnSoldados()
    {
        if (prefabSoldado == null || pontosSpawnSoldado.Length == 0) return;

        foreach (Transform ponto in pontosSpawnSoldado)
        {
            if (ponto == null) continue;

            GameObject soldado = Instantiate(prefabSoldado, ponto.position, Quaternion.identity);
            Rigidbody2D rb = soldado.GetComponent<Rigidbody2D>();
            if (rb != null && forcaSpawnVertical > 0f)
                rb.AddForce(Vector2.up * forcaSpawnVertical, ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// Ignora a colisão entre o objeto de ataque e um array de Colliders.
    /// </summary>
    void IgnoreCollisionWithObject(GameObject objToIgnore, Collider2D[] targetColliders)
    {
        if (objToIgnore == null || targetColliders.Length == 0) return;

        Collider2D[] projColliders = objToIgnore.GetComponentsInChildren<Collider2D>();

        foreach (var projCol in projColliders)
        {
            if (projCol == null) continue;
            foreach (var targetCol in targetColliders)
            {
                if (targetCol != null)
                {
                    Physics2D.IgnoreCollision(projCol, targetCol, true);
                }
            }
        }
    }

    // --- Gizmos ---
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raioAtivacao);

        if (pontosSpawnSoldado != null)
        {
            Gizmos.color = Color.yellow;
            foreach (var ponto in pontosSpawnSoldado)
                if (ponto != null)
                    Gizmos.DrawSphere(ponto.position, 0.15f);
        }

        if (PontoTiroHiller != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(PontoTiroHiller.position, 0.15f);
        }
    }
}