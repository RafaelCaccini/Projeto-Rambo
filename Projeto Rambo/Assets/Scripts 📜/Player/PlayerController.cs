using TMPro;

using Unity.Cinemachine;

using UnityEngine;

using UnityEngine.InputSystem;

using UnityEngine.SceneManagement;

using System.Collections;



public class PlayerController : MonoBehaviour

{

    // Adicione esses campos para os Animators

    [Header("Animações")]

    public Animator animatorCorpo;

    public Animator animatorPerna;



    public float moveX;



    [Header("Movimentação")]

    public float velocidadeNormal = 5f;

    public float velocidadeAgachado = 2f;



    [Header("Referências")]

    public Transform cuboSuperior;

    public Transform cuboInferior;

    public GameObject groundDetector;

    private LifeScript lifeScript; // referência ao script de vida



    [Header("Agachamento")]

    public float alturaAgachado = 0.5f;

    public float alturaNormal = 1f;



    [Header("Pulo")]

    public float forcaPulo = 5f;

    public LayerMask groundLayer;



    [Header("Tiro")]

    public GameObject prefabTiro;

    public float velocidadeTiro = 10f;

    public Transform pontoDisparo; // Ponto de onde o tiro e a granada saem



    [Header("Granada")]

    public GameObject prefabGranada; // O objeto que será lançado

    public float forcaLancamentoGranada = 5f; // Força do arremesso da granada

    public float cooldownGranada = 1f; // Tempo de espera entre os lançamentos

    public int granadasRestantes = 3; // Quantidade de granadas que o jogador começa

    private float proximoLançamentoGranada;



    private bool estaAgachado = false;

    private bool podePular = false;

    private Rigidbody2D rb;

    private bool olhandoParaDireita = true;

    private Vector3 posicaoOriginalCuboSuperior;

    private Vector3 posicaoAgachadoCuboSuperior;



    // Nova variável para contar contatos com o chão

    private int contatoComChao = 0;



    // hashes para deixar as ações mais rapidas e leves

    private int movendoHash = Animator.StringToHash("Movendo");

    private int saltandoHash = Animator.StringToHash("Saltando");

    private int movendoCimaHash = Animator.StringToHash("MovendoCima");

    private int atirandoHash = Animator.StringToHash("Atirando");

    private int atirandoCimaHash = Animator.StringToHash("AtirandoCima");

    private int olhandoCimaHash = Animator.StringToHash("OlhandoCima");

    private int granadaHash = Animator.StringToHash("Granada");

    private int agacharHash = Animator.StringToHash("Agachado");

    private int andandoAgachadoHash = Animator.StringToHash("AndandoAgachado");





    [Header("Especial")]

    public bool especial = false;

    public float raioDoEspecial = 5f; // Raio de alcance do especial



    void Start()

    {

        rb = GetComponent<Rigidbody2D>();

        lifeScript = GetComponent<LifeScript>();



        posicaoOriginalCuboSuperior = cuboSuperior.localPosition;

        posicaoAgachadoCuboSuperior = new Vector3(

          posicaoOriginalCuboSuperior.x,

          posicaoOriginalCuboSuperior.y - 0.2f,

          posicaoOriginalCuboSuperior.z

        );

    }



    void Update()

    {

        Mover();

        Agachar();

        Pular();



        // Lógica para a animação de olhar para cima

        if (Keyboard.current.wKey.isPressed)

        {

            animatorCorpo.SetBool(olhandoCimaHash, true);

        }

        else

        {

            animatorCorpo.SetBool(olhandoCimaHash, false);

        }



        // Lógica para a animação de tiro lateral

        if (Keyboard.current.eKey.wasPressedThisFrame)

        {

            animatorCorpo.SetBool(atirandoHash, true);

            Atirar();

        }

        else

        {

            animatorCorpo.SetBool(atirandoHash, false);

        }



        // Lógica para a animação de atirar para cima

        if (Keyboard.current.eKey.isPressed && Keyboard.current.wKey.isPressed)

        {

            animatorCorpo.SetBool(atirandoCimaHash, true);

        }

        else

        {

            animatorCorpo.SetBool(atirandoCimaHash, false);

        }



        if (Keyboard.current.fKey.wasPressedThisFrame)

        {

            animatorCorpo.SetTrigger(granadaHash);

            LancarGranada();

        }



        if (Keyboard.current.qKey.wasPressedThisFrame && especial)

        {

            LancarEspecial(transform.position, raioDoEspecial);

            especial = false;

        }



        if (lifeScript != null && lifeScript.GetVidaAtual() <= 0)

        {

            SceneManager.LoadScene("Morte");

        }

    }
    // Adicione esta nova função ao seu script PlayerController
    public void AtivarEscudo(float duracao)
    {
        if (lifeScript != null)
        {
            lifeScript.ignorarDano = true; // Ativa o escudo
            StartCoroutine(DesativarEscudo(duracao)); // Inicia a corotina a partir do PlayerController
        }
    }

    // Coroutine para desativar o escudo após um tempo determinado
    private IEnumerator DesativarEscudo(float duracao)
    {
        yield return new WaitForSeconds(duracao);
        if (lifeScript != null)
        {
            lifeScript.ignorarDano = false; // Desativa o escudo
        }
    }

    // ... o restante do seu script PlayerController


    void Mover()

    {

        float velocidade = estaAgachado ? velocidadeAgachado : velocidadeNormal;

        moveX = 0f;



        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)

        {

            moveX = -1f;

            olhandoParaDireita = false;

        }

        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)

        {

            moveX = 1f;

            olhandoParaDireita = true;

        }



        rb.linearVelocity = new Vector2(moveX * velocidade, rb.linearVelocity.y);



        // Lógica para as animações de movimento

        if (moveX != 0)

        {

            if (estaAgachado)

            {

                // Anda agachado

                animatorPerna.SetBool(andandoAgachadoHash, true);

                animatorPerna.SetBool(movendoHash, false);

            }

            else

            {

                // Anda normalmente

                animatorPerna.SetBool(andandoAgachadoHash, false);

                animatorPerna.SetBool(movendoHash, true);

            }

            // A animação do corpo movendo-se para cima pode ser independente

            animatorCorpo.SetBool(movendoCimaHash, true);

        }

        else

        {

            // Para de se mover

            animatorPerna.SetBool(andandoAgachadoHash, false);

            animatorPerna.SetBool(movendoHash, false);

            animatorCorpo.SetBool(movendoCimaHash, false);

        }



        Vector3 escala = transform.localScale;

        escala.x = olhandoParaDireita ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);

        transform.localScale = escala;

    }



    void Pular()

    {

        if (Keyboard.current.spaceKey.wasPressedThisFrame && podePular && !estaAgachado)

        {

            rb.AddForce(Vector2.up * forcaPulo, ForceMode2D.Impulse);

            podePular = false;

            animatorPerna.SetBool(saltandoHash, true);

        }

    }



    void Agachar()

    {

        bool agachar = Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed;



        // Ativa ou desativa a animação de agachar

        animatorPerna.SetBool(agacharHash, agachar);



        if (agachar && !estaAgachado)

        {

            estaAgachado = true;

            cuboSuperior.localPosition = posicaoAgachadoCuboSuperior;

        }

        else if (!agachar && estaAgachado)

        {



            estaAgachado = false;

            cuboSuperior.localPosition = posicaoOriginalCuboSuperior;

        }

    }



    void Atirar()

    {

        if (prefabTiro == null || pontoDisparo == null)

        {

            Debug.LogWarning("Prefab do Tiro ou Ponto de Disparo não estão configurados.");

            return;

        }



        Vector2 direcao;

        Vector3 posicaoDisparo = pontoDisparo.position;

        Quaternion rotacaoTiro = Quaternion.identity;



        if (Keyboard.current.wKey.isPressed)

        {

            animatorCorpo.SetBool(atirandoCimaHash, true);

            direcao = Vector2.up;

            posicaoDisparo = cuboSuperior.position + new Vector3(0f, 0.7f, 0f);

            rotacaoTiro = Quaternion.Euler(0, 0, 90);



        }

        else

        {



            direcao = olhandoParaDireita ? Vector2.right : Vector2.left;

            if (!olhandoParaDireita)

                rotacaoTiro = Quaternion.Euler(0, 0, 180);



        }



        GameObject tiro = Instantiate(prefabTiro, posicaoDisparo, rotacaoTiro);

        tiro.tag = "Danger";



        Rigidbody2D rbTiro = tiro.GetComponent<Rigidbody2D>();

        if (rbTiro != null)

            rbTiro.linearVelocity = direcao * velocidadeTiro;

    }



    void LancarGranada()

    {

        if (Time.time < proximoLançamentoGranada || granadasRestantes <= 0)

        {

            return;

        }



        granadasRestantes--;

        proximoLançamentoGranada = Time.time + cooldownGranada;



        if (prefabGranada == null || pontoDisparo == null)

        {

            Debug.LogWarning("Prefab da Granada ou Ponto de Disparo não estão configurados.");

            return;

        }



        GameObject granada = Instantiate(prefabGranada, pontoDisparo.position, Quaternion.identity);



        Rigidbody2D rbGranada = granada.GetComponent<Rigidbody2D>();

        if (rbGranada != null)

        {

            Vector2 forcaLancamento;

            if (olhandoParaDireita)

            {

                forcaLancamento = new Vector2(1, 1).normalized * forcaLancamentoGranada;

            }

            else

            {

                forcaLancamento = new Vector2(-1, 1).normalized * forcaLancamentoGranada;

            }



            rbGranada.AddForce(forcaLancamento, ForceMode2D.Impulse);

        }

    }



    public void LancarEspecial(Vector2 centro, float raio)

    {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(centro, raio);



        foreach (Collider2D col in colliders)

        {

            if (col.CompareTag("Enemy"))

            {

                Destroy(col.gameObject);

            }

        }

    }



    void OnCollisionEnter2D(Collision2D collision)

    {

        // Se a colisão for com o chão

        if (((1 << collision.gameObject.layer) & groundLayer) != 0)

        {

            contatoComChao++;

            podePular = (contatoComChao > 0);

            animatorPerna.SetBool(saltandoHash, false);

        }

    }



    void OnCollisionExit2D(Collision2D collision)

    {

        // Se sair da colisão com o chão

        if (((1 << collision.gameObject.layer) & groundLayer) != 0)

        {

            contatoComChao--;

            podePular = (contatoComChao > 0);

        }

    }

}