using UnityEngine;
using System.Collections;

public class Espartano : MonoBehaviour
{
    [Header("Referências")]
    public Transform jogador;
    public Animator animator;
    public Collider2D zonaAtaque;

    [Header("Atributos")]
    public float velocidade = 2f;
    public float distanciaMinima = 1.5f;
    public float distanciaDeteccao = 8f;
    public float tempoEntreAtaques = 2f;
    public int danoAtaque = 10;
    public float delayVirar = 0.5f;

    [Header("Áudio")]
    public AudioSource audioSource;   // 👉 Arrasta o AudioSource aqui
    public AudioClip somLanca;        // 👉 E aqui o som da lança

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
        // Se não puder andar → fica no idle
        if (!podeAndar)
        {
            animator.SetBool("Andando", false);
            return;
        }

        // Verifica lado do jogador
        bool jogadorADireita = jogador.position.x > transform.position.x;

        // Se estiver virado errado → parar e esperar virar
        if (jogadorADireita != viradoDireita && podeVirar)
        {
            animator.SetBool("Andando", false);
            StartCoroutine(VirarComDelay());
            return;
        }

        // Agora pode andar
        animator.SetBool("Andando", true);

        float direcao = viradoDireita ? 1f : -1f;
        transform.Translate(Vector2.right * direcao * velocidade * Time.deltaTime);
    }



    void Atacar()
    {
        if (contadorAtaque > 0f) return;

        contadorAtaque = tempoEntreAtaques;
        animator.SetTrigger("Atacar");

        // toca o som da lança
        if (audioSource != null && somLanca != null)
            audioSource.PlayOneShot(somLanca);

        StartCoroutine(PausarMovimento(0.5f));
    }

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

        animator.SetBool("Andando", false); // força idle

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
