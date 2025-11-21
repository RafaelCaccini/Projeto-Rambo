using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ChefeHiller : MonoBehaviour
{
    private Animator animator;
    public Transform jogador;

    [Header("Tiro do Chefe")]
    public Transform PontoTiroVisual;
    public Transform PontoTiroAtaque;
    public GameObject prefabProjeteil;
    public GameObject prefabProjetilVisual;

    [Header("Configurações de Tiro")]
    public float offsetForward = 0.2f;
    public float velocidadeTiro = 10f;
    public float velocidadeTiroVisual = 5f;
    public float cooldownTiro = 2f;

    [Header("Soldados")]
    public Transform[] pontosSpawnSoldado;
    public GameObject prefabSoldado;
    public float cooldownSpawnSoldado = 5f;
    public float forcaSpawnVertical = 5f;

    [Header("Outros")]
    public GameObject chaoDoHillerRoot;
    public float raioAtivacao = 5f;
    public int contagemAtingidoPorEspecial = 0;
    public const int LIMITE_ESPECIAL = 2;
    public string nomeCenaMorte;
    public float delayAntesMorte = 2f;

    [Header("Som do Disparo")]
    public AudioSource audioSourceDisparo;
    public AudioSource musica;

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
        {
            ativo = true;
            playMusica();
        }
        
        

        if (!ativo) return;

        timerTiro -= Time.deltaTime;
        if (timerTiro <= 0f)
        {
            animator?.SetTrigger(AnimAtirar);
            timerTiro = cooldownTiro;
        }

        timerSpawnSoldado -= Time.deltaTime;
        if (timerSpawnSoldado <= 0f)
        {
            SpawnSoldados();
            timerSpawnSoldado = cooldownSpawnSoldado;
        }
    }

    public void ReceberDanoEspecial()
    {
        contagemAtingidoPorEspecial++;
        Debug.Log($"Boss Hiller atingido pelo Especial. Contagem atual: {contagemAtingidoPorEspecial}");

        if (contagemAtingidoPorEspecial >= LIMITE_ESPECIAL)
        {
            Debug.Log("Boss Hiller derrotado! Preparando animação de morte...");
            ativo = false;
            StopAllCoroutines();
            StartCoroutine(EsperarEIniciarMorte());
        }
    }

    private IEnumerator EsperarEIniciarMorte()
    {
        yield return new WaitForSeconds(delayAntesMorte);
        animator?.SetTrigger("InimigoMorrendo");
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
        if (prefabProjeteil == null || jogador == null || PontoTiroAtaque == null)
            return;

        // Direção e rotação
        Vector2 direcao = (jogador.position - PontoTiroAtaque.position).normalized;
        Vector3 spawnPos = PontoTiroAtaque.position + (Vector3)direcao * offsetForward;
        float angle = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0f, 0f, angle);

        // Instancia o projétil
        GameObject proj = Instantiate(prefabProjeteil, spawnPos, rot);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direcao * velocidadeTiro;

        // Ignora colisões
        IgnoreCollisionWithObject(proj, hillerColliders);
        if (chaoDoHillerRoot != null)
        {
            Collider2D[] chaoColliders = chaoDoHillerRoot.GetComponentsInChildren<Collider2D>();
            IgnoreCollisionWithObject(proj, chaoColliders);
        }

        // 🎯 Toca o som direto no AudioSource arrastado
        if (audioSourceDisparo != null)
        {
            audioSourceDisparo.Stop(); // garante que reinicia o som
            audioSourceDisparo.Play();
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

    public void FinalizarMorte()
    {
        StartCoroutine(DestruirEDepoisTrocarCena());
    }

    private IEnumerator DestruirEDepoisTrocarCena()
    {
        yield return new WaitForSeconds(0.2f);
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
    public void playMusica()
    {
        musica.Play();
    }
    public void stopMusica()
    {
        musica.Stop();
    }
}
