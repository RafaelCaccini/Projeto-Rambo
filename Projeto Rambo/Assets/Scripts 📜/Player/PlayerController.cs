using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movimentação")]
    public float velocidadeNormal = 5f;
    public float velocidadeAgachado = 2f;

    [Header("Referências")]
    public Transform cuboSuperior;
    public Transform cuboInferior;
    // O groundDetector não será mais necessário com esta abordagem
    public GameObject groundDetector;

    [Header("Agachamento")]
    public float alturaAgachado = 0.5f;
    public float alturaNormal = 1f;

    [Header("Pulo")]
    public float forcaPulo = 5f;
    public LayerMask groundLayer;

    private bool estaAgachado = false;
    private bool podePular = false; // Começamos com false, e só ativamos na primeira colisão
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Note: o groundDetector não é mais necessário aqui.
        // O próprio BoxCollider2D do personagem será usado para a colisão.
    }

    void Update()
    {
        Mover();
        Agachar();
        Pular();
    }

    // Removido FixedUpdate, já que a lógica de pulo é baseada em eventos
    void Mover()
    {
        float velocidade = estaAgachado ? velocidadeAgachado : velocidadeNormal;

        float moveX = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            moveX = -1f;
        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            moveX = 1f;

        rb.linearVelocity = new Vector2(moveX * velocidade, rb.linearVelocity.y);
    }

    void Pular()
    {
        if (Keyboard.current.wKey.wasPressedThisFrame && podePular)
        {
            rb.AddForce(Vector2.up * forcaPulo, ForceMode2D.Impulse);
            // Imediatamente desativa a habilidade de pular
            podePular = false;
        }
    }

    void Agachar()
    {
        bool agachar = Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed;

        if (agachar)
        {
            if (!estaAgachado)
            {
                estaAgachado = true;
                Vector3 pos = cuboSuperior.localPosition;
                pos.y = alturaAgachado;
                cuboSuperior.localPosition = pos;
            }
        }
        else
        {
            if (estaAgachado)
            {
                estaAgachado = false;
                Vector3 pos = cuboSuperior.localPosition;
                pos.y = alturaNormal;
                cuboSuperior.localPosition = pos;
            }
        }
    }

    // **NOVA LÓGICA DE DETECÇÃO DE CHÃO**

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Se a colisão for com um objeto na layer de chão, ativamos o pulo
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            podePular = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Se a colisão era com o chão, desativamos o pulo
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            podePular = false;
        }
    }
}