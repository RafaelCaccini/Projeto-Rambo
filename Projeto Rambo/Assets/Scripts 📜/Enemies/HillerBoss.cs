using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ChefeHiller : MonoBehaviour
{
    private Animator animator; // controla as animações do chefe

    // referência ao jogador pra saber onde ele está
    public Transform jogador;

    // pontos de onde os tiros saem e os prefabs dos projéteis
    public Transform PontoTiroVisual;     // tiro "falso" que sobe
    public Transform PontoTiroAtaque;     // tiro verdadeiro que causa dano
    public GameObject prefabProjeteil;
    public GameObject prefabProjetilVisual;

    // ajustes da velocidade e tempo dos tiros
    public float offsetForward = 0.2f;
    public float velocidadeTiro = 10f;
    public float velocidadeTiroVisual = 5f;
    public float cooldownTiro = 2f;

    // spawn de soldados inimigos
    public Transform[] pontosSpawnSoldado;
    public GameObject prefabSoldado;
    public float cooldownSpawnSoldado = 5f;
    public float forcaSpawnVertical = 5f;

    // objeto do chão do chefe (usado pra ignorar colisão dos tiros)
    public GameObject chaoDoHillerRoot;

    // quando o jogador chega perto, ativa o chefe
    public float raioAtivacao = 5f;

    // controle de dano especial — se tomar 2 golpes especiais, morre
    public int contagemAtingidoPorEspecial = 0;
    public const int LIMITE_ESPECIAL = 2;

    private readonly int AnimAtirar = Animator.StringToHash("Atirar"); // otimiza trigger

    private float timerTiro = 0f;
    private float timerSpawnSoldado = 0f;
    private bool ativo = false;
    private Vector3 initialScale;
    private Collider2D[] hillerColliders;
    public string nomeCenaMorte;     // nome da cena que será carregada
    public float delayAntesMorte = 2f; // tempo de espera em segundos

    void Start()
    {
        animator = GetComponent<Animator>();

        // garante que o chefe começa virado pro lado certo
        initialScale = transform.localScale;
        transform.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);

        // inicia contadores de tempo
        timerTiro = cooldownTiro;
        timerSpawnSoldado = cooldownSpawnSoldado;

        // pega todos os colliders pra ignorar colisão com os tiros depois
        hillerColliders = GetComponentsInChildren<Collider2D>();
    }

    void Update()
    {
        if (jogador == null) return;

        // ativa o chefe quando o jogador chega perto
        if (!ativo && Vector2.Distance(transform.position, jogador.position) <= raioAtivacao)
            ativo = true;

        if (!ativo) return;

        // controla tempo entre os tiros e dispara quando chega a zero
        timerTiro -= Time.deltaTime;
        if (timerTiro <= 0f)
        {
            if (animator != null)
                animator.SetTrigger(AnimAtirar);
            timerTiro = cooldownTiro;
        }

        // controla tempo entre spawn de soldados
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
            Debug.Log("Boss Hiller foi derrotado pelo Especial! Carregando cena 'Mapa'.");

            Destroy(gameObject); // remove o chefe da cena
            SceneManager.LoadScene("Mapa"); // carrega a próxima cena
        }
    }

    // cria um projétil visual que só sobe (não causa dano)
    public void DispararTiroVisual()
    {
        if (PontoTiroVisual == null || prefabProjetilVisual == null) return;

        Quaternion verticalRotation = Quaternion.Euler(0, 0, 90);
        GameObject visualProj = Instantiate(prefabProjetilVisual, PontoTiroVisual.position, verticalRotation);

        Rigidbody2D visualRb = visualProj.GetComponent<Rigidbody2D>();
        if (visualRb != null)
            visualRb.linearVelocity = Vector2.up * velocidadeTiroVisual;

        Destroy(visualProj, 1.5f); // destrói o tiro depois de 1.5s
    }

    // cria o projétil real que vai em direção ao jogador
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

            // posição inicial um pouco à frente
            Vector3 spawnPos = PontoTiroAtaque.position + (Vector3)direcao * offsetForward;

            float angle = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(0f, 0f, angle);

            GameObject proj = Instantiate(prefabProjeteil, spawnPos, rot);

            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = direcao * velocidadeTiro;

            // ignora colisão dos tiros com o próprio chefe e com o chão dele
            IgnoreCollisionWithObject(proj, hillerColliders);
            if (chaoDoHillerRoot != null)
            {
                Collider2D[] chaoColliders = chaoDoHillerRoot.GetComponentsInChildren<Collider2D>();
                IgnoreCollisionWithObject(proj, chaoColliders);
            }
        }
    }

    // cria soldados nos pontos configurados
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

    // impede que projéteis colidam com certos objetos
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

    // desenha no editor o raio de ativação e pontos de spawn
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

    public void FinalizarMorte()
    {
        StartCoroutine(DestruirEDepoisTrocarCena());
    }

    private IEnumerator DestruirEDepoisTrocarCena()
    {
        // espera o tempo configurado
        yield return new WaitForSeconds(delayAntesMorte);

        // destrói o boss
        Destroy(gameObject);

        // carrega a cena se o nome estiver definido
        if (!string.IsNullOrEmpty(nomeCenaMorte))
        {
            SceneManager.LoadScene(nomeCenaMorte);
        }
    }
}
