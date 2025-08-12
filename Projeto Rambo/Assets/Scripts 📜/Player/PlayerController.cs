using UnityEngine;
using UnityEngine.InputSystem; // Novo sistema de Input

public class PlayerController : MonoBehaviour
{
    [Header("Movimentação")]
    public float velocidadeNormal = 5f;
    public float velocidadeAgachado = 2f;

    [Header("Referências")]
    public Transform cuboSuperior;
    public Transform cuboInferior;
    public GameObject groundDetector; // Objeto filho com BoxCollider2D nos pés

    [Header("Agachamento")]
    public float alturaAgachado = 0.5f;
    public float alturaNormal = 1f;

    private bool estaAgachado = false;
    private Rigidbody2D rb;
    private Collider2D groundCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (groundDetector != null)
        {
            groundCollider = groundDetector.GetComponent<Collider2D>();
            if (groundCollider == null)
            {
                Debug.LogError("O objeto groundDetector precisa ter um Collider2D!");
            }
        }
        else
        {
            Debug.LogError("GroundDetector não atribuído no Inspector!");
        }
    }

    void Update()
    {
        Mover();
        Agachar();
        Pular();
    }

    void Mover()
    {
        float velocidade = estaAgachado ? velocidadeAgachado : velocidadeNormal;

        // Novo sistema de Input
        float moveX = 0f;
        float moveY = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            moveX = -1f;
        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            moveX = 1f;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            moveY = 1f;
        else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            moveY = -1f;

        // Se estiver agachado, move só para os lados
        if (estaAgachado)
            moveY = 0f;

        Vector2 movimento = new Vector2(moveX, moveY) * velocidade;

        rb.linearVelocity = new Vector2(movimento.x, rb.linearVelocity.y);
    }
    void Pular()
    {
        // Novo sistema de Input
        if (Keyboard.current.wKey.wasPressedThisFrame && EstaNoChao())
        {
            rb.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
        }
    }


    void Agachar()
    {
        // Novo sistema de Input
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

    public bool EstaNoChao()
    {
        if (groundCollider == null) return false;

        // Pega os dados do BoxCollider2D
        var box = groundCollider as BoxCollider2D;
        if (box == null) return false;

        Vector2 centro = box.bounds.center;
        Vector2 tamanho = box.size * box.transform.lossyScale;

        // Checa se está sobrepondo algum chão na layer Default
        return Physics2D.OverlapBox(centro, tamanho, box.transform.eulerAngles.z, LayerMask.GetMask("Default")) != null;
    }

}

