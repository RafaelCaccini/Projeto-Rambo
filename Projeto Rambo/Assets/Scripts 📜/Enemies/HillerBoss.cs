using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // <<== NOVO: NECESSÁRIO PARA CARREGAR CENAS

public class ChefeHiller : MonoBehaviour
{
    private Animator animator;

    [Header("Referências")]
    public Transform jogador;

    [Header("Spawn / Projétil")]
    [Tooltip("Ponto de onde o projétil visual (que só sobe) sai da arma.")]
    public Transform PontoTiroVisual;
    [Tooltip("Ponto FORA DA TELA de onde o projétil de ataque (o verdadeiro) sai.")]
    public Transform PontoTiroAtaque;
    public GameObject prefabProjeteil;
    [Tooltip("O prefab do projétil que APENAS sobe e é destruído (não causa dano).")]
    public GameObject prefabProjetilVisual;

    [Header("Ajustes de Projétil")]
    public float offsetForward = 0.2f;
    public float velocidadeTiro = 10f;
    public float velocidadeTiroVisual = 5f;
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

    // Controle de Dano
    [Header("Controle de Dano")]
    [Tooltip("O Boss morre quando esta contagem atinge o limite.")]
    public int contagemAtingidoPorEspecial = 0;
    public const int LIMITE_ESPECIAL = 2; // Constante para o número de hits

    // Hash para o Trigger de animação
    private readonly int AnimAtirar = Animator.StringToHash("Atirar");

    private float timerTiro = 0f;
    private float timerSpawnSoldado = 0f;
    private bool ativo = false;
    private Vector3 initialScale;

    private Collider2D[] hillerColliders;

    void Start()
    {
        animator = GetComponent<Animator>();

        initialScale = transform.localScale;
        transform.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);

        timerTiro = cooldownTiro;
        timerSpawnSoldado = cooldownSpawnSoldado;

        hillerColliders = GetComponentsInChildren<Collider2D>();
    }

    void Update()
    {
        if (jogador == null) return;

        if (!ativo && Vector2.Distance(transform.position, jogador.position) <= raioAtivacao)
            ativo = true;

        if (!ativo) return;

        timerTiro -= Time.deltaTime;
        if (timerTiro <= 0f)
        {
            if (animator != null)
            {
                animator.SetTrigger(AnimAtirar);
            }
            timerTiro = cooldownTiro;
        }

        timerSpawnSoldado -= Time.deltaTime;
        if (timerSpawnSoldado <= 0f)
        {
            SpawnSoldados();
            timerSpawnSoldado = cooldownSpawnSoldado;
        }
    }

    // ===============================================
    // MÉTODO DE DANO COM LÓGICA DE MORTE
    // ===============================================
    public void ReceberDanoEspecial()
    {
        contagemAtingidoPorEspecial++;
        Debug.Log($"Boss Hiller atingido pelo Especial. Contagem atual: {contagemAtingidoPorEspecial}");

        if (contagemAtingidoPorEspecial >= LIMITE_ESPECIAL)
        {
            Debug.Log("Boss Hiller foi derrotado pelo Especial! Carregando cena 'Mapa'.");

            // 1. Destrói o Boss
            Destroy(gameObject);

            // 2. Carrega a nova cena
            SceneManager.LoadScene("Mapa"); // <<== AÇÃO PRINCIPAL
        }
    }

    // --- DISPARO VISUAL (FALSO) ---
    public void DispararTiroVisual()
    {
        if (PontoTiroVisual == null || prefabProjetilVisual == null) return;

        GameObject visualProj = Instantiate(prefabProjetilVisual, PontoTiroVisual.position, Quaternion.identity);

        Rigidbody2D visualRb = visualProj.GetComponent<Rigidbody2D>();
        if (visualRb != null)
        {
            visualRb.linearVelocity = Vector2.up * velocidadeTiroVisual;
        }

        Destroy(visualProj, 1.5f);
    }

    // --- DISPARO DE ATAQUE (REAL) ---
    public void DispararTiroAtaque()
    {
        if (prefabProjeteil == null || jogador == null)
        {
            Debug.LogError("Configuração de projétil incompleta.");
            return;
        }

        if (PontoTiroAtaque != null)
        {
            Vector2 direcao = (jogador.position - PontoTiroAtaque.position).normalized;

            Vector3 spawnPos = PontoTiroAtaque.position + (Vector3)direcao * offsetForward;

            float angle = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(0f, 0f, angle);

            GameObject proj = Instantiate(prefabProjeteil, spawnPos, rot);

            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = direcao * velocidadeTiro;
            }

            IgnoreCollisionWithObject(proj, hillerColliders);
            if (chaoDoHillerRoot != null)
            {
                Collider2D[] chaoColliders = chaoDoHillerRoot.GetComponentsInChildren<Collider2D>();
                IgnoreCollisionWithObject(proj, chaoColliders);
            }
        }
    }


    // --- Spawn de Soldados e Funções Auxiliares (Não alteradas) ---
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

        if (PontoTiroVisual != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(PontoTiroVisual.position, 0.15f);
            Gizmos.DrawRay(PontoTiroVisual.position, Vector2.up * 1f);
        }

        if (PontoTiroAtaque != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(PontoTiroAtaque.position, 0.3f);
        }
    }
}