using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; // necessário pra trocar de cena

public class PlayerController : MonoBehaviour
{
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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lifeScript = GetComponent<LifeScript>(); // pega o script de vida do mesmo objeto

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

        // Ativa o método de tiro na tecla E
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Atirar();
        }

        // Ativa o método de lançamento de granada na tecla F
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            LancarGranada();
        }

        // Se a vida chegou a 0, troca de cena
        if (lifeScript != null && lifeScript.GetVidaAtual() <= 0)
        {
            SceneManager.LoadScene("Morte");
        }
    }

    void Mover()
    {
        float velocidade = estaAgachado ? velocidadeAgachado : velocidadeNormal;
        float moveX = 0f;

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

        Vector3 escala = transform.localScale;
        escala.x = olhandoParaDireita ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);
        transform.localScale = escala;
    }

    void Pular()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && podePular)
        {
            rb.AddForce(Vector2.up * forcaPulo, ForceMode2D.Impulse);
            podePular = false;
        }
    }

    void Agachar()
    {
        bool agachar = Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed;

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
            direcao = Vector2.up;
            posicaoDisparo = cuboSuperior.position + new Vector3(0f, 0.5f, 0f);
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
        // Verifica se o tempo de espera da granada já passou E se ainda existem granadas
        if (Time.time < proximoLançamentoGranada || granadasRestantes <= 0)
        {
            return;
        }

        // Diminui a contagem de granadas
        granadasRestantes--;

        // Define o tempo para o próximo lançamento de granada
        proximoLançamentoGranada = Time.time + cooldownGranada;

        if (prefabGranada == null || pontoDisparo == null)
        {
            Debug.LogWarning("Prefab da Granada ou Ponto de Disparo não estão configurados.");
            return;
        }

        // Instancia a granada no ponto de disparo
        GameObject granada = Instantiate(prefabGranada, pontoDisparo.position, Quaternion.identity);

        Rigidbody2D rbGranada = granada.GetComponent<Rigidbody2D>();
        if (rbGranada != null)
        {
            // Define a força inicial, combinando a direção horizontal com um pulo vertical
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            podePular = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {

        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            podePular = false;
        }
    }
}