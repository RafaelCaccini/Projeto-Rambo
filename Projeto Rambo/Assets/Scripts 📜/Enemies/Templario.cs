using UnityEngine;





public class Templario : MonoBehaviour

{

    [Header("Configurações de Movimento")]

    public float velocidade = 3f; // velocidade que o Templário anda

    private Rigidbody2D rb; // referência do Rigidbody para movimentação



    [Header("Referências")]

    [Tooltip("Arraste e solte o objeto do jogador neste campo.")]

    public Transform jogadorTransform; // referência do jogador



    [Header("Configurações de Ataque")]

    public GameObject objetoDeDanoPrefab; // objeto que causa dano quando ataca

    private bool estaAtacando = false; // se ele está atacando



    [Header("Detecção do Jogador")]

    [Tooltip("O raio da área de detecção do jogador.")]

    public float raioDeDeteccao = 5f; // alcance para detectar o jogador

    private bool alvoDetectado = false; // se já viu o jogador

    public float TempoDano = 0.05f; // tempo que o objeto de dano fica ativo



    public Animator Animator; // referência ao Animator para controlar animações



    void Start()

    {

        rb = GetComponent<Rigidbody2D>(); // pega o Rigidbody

    }



    void FixedUpdate()

    {

        if (jogadorTransform == null) return; // se não tiver jogador, não faz nada



        // procura o jogador dentro do raio

        if (!alvoDetectado)

        {

            float distancia = Vector2.Distance(transform.position, jogadorTransform.position);

            if (distancia <= raioDeDeteccao)

            {

                alvoDetectado = true; // agora viu o jogador

                Debug.Log("JOGADOR DETECTADO! O Templário vai começar a se mover.");

                Animator.SetBool("Correndo", true); // ativa a animação de andar

            }

        }



        // se viu o jogador e não está atacando, anda para frente

        if (alvoDetectado && !estaAtacando)

        {
            Animator.SetBool("Correndo", true); // ativa anim de corrida
            rb.AddForce(Vector2.left * velocidade * 5f); // aplica força para mover

        }

        else

        {

            rb.linearVelocity = Vector2.zero; // para o movimento

        }

    }



    // --- Colisões e Ataque ---

    private void OnCollisionEnter2D(Collision2D collision)

    {

        if (collision.gameObject.CompareTag("Player"))

        {

            rb.linearVelocity = Vector2.zero; // para o movimento ao bater no jogador



            estaAtacando = true; // começa o ataque



            RealizarAtaque(); // chama o ataque

        }

    }



    // função que cria o objeto de dano

    private void RealizarAtaque()

    {
        
        if (objetoDeDanoPrefab != null)

        {
            Animator.SetTrigger("Atacando"); // inicia a animação de ataque

            Debug.Log("Realizando Ataque! Objeto de dano instanciado.");



            Animator.SetTrigger("Atacando"); // inicia a animação de ataque

            GameObject objetoDeDano = Instantiate(objetoDeDanoPrefab, transform.position, Quaternion.identity);
            objetoDeDano.tag = "Danger"; // marca o objeto como perigoso



            Destroy(objetoDeDano, 0.5f); // destrói rapidinho

            Animator.SetBool("Preso", true); // desativa a animação de andar

        }

    }



    private void OnDrawGizmosSelected()

    {

        // mostra no editor o alcance de detecção

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, raioDeDeteccao);

    }

}