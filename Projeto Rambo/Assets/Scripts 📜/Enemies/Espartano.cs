using UnityEngine;
using System.Collections;

public class Espartano : MonoBehaviour
{
    [Header("Referências")]
    public Transform jogador;
    public Animator animator;
    public Collider2D zonaAtaque;    // Trigger de ataque

    [Header("Atributos")]
    public float velocidade = 2f;
    public float distanciaMinima = 1.5f;
    public float distanciaDeteccao = 8f;
    public float tempoEntreAtaques = 2f;
    public int danoAtaque = 10;
    public float delayVirar = 0.5f;

    private bool viradoDireita = true;
    private bool podeVirar = true;
    private bool podeAndar = true;
    private float contadorAtaque = 0f;

    void Start()
    {
        if (jogador == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                jogador = player.transform;
        }
    }

    void Update()
    {
        if (jogador == null) return;

        float distancia = Vector2.Distance(transform.position, jogador.position);

        if (distancia <= distanciaDeteccao)
        {
            if (distancia > distanciaMinima)
            {
                AndarAteJogador();
            }
            else
            {
                animator.SetBool("Andando", false);
                Atacar();
            }
        }
        else
        {
            animator.SetBool("Andando", false);
        }

        contadorAtaque -= Time.deltaTime;
    }

    void AndarAteJogador()
    {
        if (!podeAndar) return; // não anda durante virada ou ataque

        animator.SetBool("Andando", true);

        // Vira pro lado certo com delay
        bool jogadorADireita = jogador.position.x > transform.position.x;
        if (jogadorADireita != viradoDireita && podeVirar)
        {
            StartCoroutine(VirarComDelay());
        }

        float direcao = viradoDireita ? 1f : -1f;
        transform.Translate(Vector2.right * direcao * velocidade * Time.deltaTime);
    }

    void Atacar()
    {
        if (contadorAtaque > 0f) return;

        contadorAtaque = tempoEntreAtaques;
        animator.SetTrigger("Atacar");

        // Durante o ataque, ele para de andar
        StartCoroutine(PausarMovimento(0.5f)); // 0.5s opcional — depende do timing da animação
    }

    // 🔹 CHAMADO POR UM ANIMATION EVENT NO MOMENTO DO IMPACTO
    public void AplicarDano()
    {
        if (zonaAtaque == null) return;

        Collider2D[] hits = Physics2D.OverlapBoxAll(zonaAtaque.bounds.center, zonaAtaque.bounds.size, 0f);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                LifeScript vidaJogador = hit.GetComponent<LifeScript>();
                if (vidaJogador != null)
                {
                    vidaJogador.TomarDano(danoAtaque);
                }
            }
        }
    }

    IEnumerator VirarComDelay()
    {
        podeVirar = false;
        podeAndar = false;
        yield return new WaitForSeconds(delayVirar);
        Virar();
        podeAndar = true;
        podeVirar = true;
    }

    IEnumerator PausarMovimento(float tempo)
    {
        podeAndar = false;
        yield return new WaitForSeconds(tempo);
        podeAndar = true;
    }

    void Virar()
    {
        viradoDireita = !viradoDireita;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    void OnDrawGizmosSelected()
    {
        if (zonaAtaque != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(zonaAtaque.bounds.center, zonaAtaque.bounds.size);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccao);
    }
}
