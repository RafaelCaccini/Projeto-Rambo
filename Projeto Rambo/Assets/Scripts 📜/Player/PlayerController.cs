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
    public Transform pontoDisparo;

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
        Atirar();

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
        if (Keyboard.current.eKey.wasPressedThisFrame && prefabTiro != null)
        {
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
