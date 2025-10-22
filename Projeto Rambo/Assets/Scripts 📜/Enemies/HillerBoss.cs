using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ChefeHiller : MonoBehaviour
{
    private Animator animator; // controla as animações do chefe
    public Transform jogador; // referência ao jogador

    public Transform PontoTiroVisual;
    public Transform PontoTiroAtaque;
    public GameObject prefabProjeteil;
    public GameObject prefabProjetilVisual;

    public float offsetForward = 0.2f;
    public float velocidadeTiro = 10f;
    public float velocidadeTiroVisual = 5f;
    public float cooldownTiro = 2f;

    public Transform[] pontosSpawnSoldado;
    public GameObject prefabSoldado;
    public float cooldownSpawnSoldado = 5f;
    public float forcaSpawnVertical = 5f;

    public GameObject chaoDoHillerRoot;

    public float raioAtivacao = 5f;
    public int contagemAtingidoPorEspecial = 0;
    public const int LIMITE_ESPECIAL = 2;

    private readonly int AnimAtirar = Animator.StringToHash("Atirar");

    private float timerTiro = 0f;
    private float timerSpawnSoldado = 0f;
    private bool ativo = false;
    private Vector3 initialScale;
    private Collider2D[] hillerColliders;

    public string nomeCenaMorte;
    public float delayAntesMorte = 2f; // tempo antes de começar animação de morte

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

        // ativa o chefe quando o jogador chega perto
        if (!ativo && Vector2.Distance(transform.position, jogador.position) <= raioAtivacao)
            ativo = true;

        if (!ativo) return;

        // controla tempo de tiro
        timerTiro -= Time.deltaTime;
        if (timerTiro <= 0f)
        {
            animator?.SetTrigger(AnimAtirar);
            timerTiro = cooldownTiro;
        }

        // controla spawn de soldados
        timerSpawnSoldado -= Time.deltaTime;
        if (timerSpawnSoldado <= 0f)
        {
            SpawnSoldados();
            timerSpawnSoldado = cooldownSpawnSoldado;
        }
    }

    // quando leva dano especial, soma e morre se passar do limite
    public void ReceberDanoEspecial()
    {
        contagemAtingidoPorEspecial++;
        Debug.Log($"Boss Hiller atingido pelo Especial. Contagem atual: {contagemAtingidoPorEspecial}");

        if (contagemAtingidoPorEspecial >= LIMITE_ESPECIAL)
        {
            Debug.Log("Boss Hiller derrotado! Preparando animação de morte...");

            ativo = false; // para o comportamento
            StopAllCoroutines(); // interrompe spawns e ataques

            StartCoroutine(EsperarEIniciarMorte());
        }
    }

    // espera X segundos e depois inicia a animação de morte
    private IEnumerator EsperarEIniciarMorte()
    {
        yield return new WaitForSeconds(delayAntesMorte);

        if (animator != null)
            animator.SetTrigger("InimigoMorrendo"); // toca a animação de morte
    }

    public void DispararTiroVisual()
    {
        if (PontoTiroVisual == null || prefabProjetilVisual == null) return;

        Quaternion verticalRotation = Quaternion.Euler(0, 0, 90);
        GameObject visualProj = Instantiate(prefabProjetilVisual, PontoTiroVisual.position, verticalRotation);

        Rigidbody2D visualRb = visualProj.GetComponent<Rigidbody2D>();
        if (visualRb != null)
            visualRb.linearVelocity = Vector2.up * velocidadeTiroVisual;

        Destroy(visualProj, 1.5f);
    }

    public void DispararTiroAtaque()
    {
        if (prefabProjeteil == null || jogador == null) return;

        if (PontoTiroAtaque != null)
        {
            Vector2 direcao = (jogador.position - PontoTiroAtaque.position).normalized;
            Vector3 spawnPos = PontoTiroAtaque.position + (Vector3)direcao * offsetForward;

            float angle = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(0f, 0f, angle);

            GameObject proj = Instantiate(prefabProjeteil, spawnPos, rot);
            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = direcao * velocidadeTiro;

            IgnoreCollisionWithObject(proj, hillerColliders);

            if (chaoDoHillerRoot != null)
            {
                Collider2D[] chaoColliders = chaoDoHillerRoot.GetComponentsInChildren<Collider2D>();
                IgnoreCollisionWithObject(proj, chaoColliders);
            }
        }
    }

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
                    Physics2D.IgnoreCollision(projCol, targetCol, true);
            }
        }
    }

    // chamado no final da animação de morte (via Animation Event)
    public void FinalizarMorte()
    {
        StartCoroutine(DestruirEDepoisTrocarCena());
    }

    private IEnumerator DestruirEDepoisTrocarCena()
    {
        yield return new WaitForSeconds(0.2f); // pequeno tempo pra garantir transição

        Destroy(gameObject);

        if (!string.IsNullOrEmpty(nomeCenaMorte))
            SceneManager.LoadScene(nomeCenaMorte);
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
