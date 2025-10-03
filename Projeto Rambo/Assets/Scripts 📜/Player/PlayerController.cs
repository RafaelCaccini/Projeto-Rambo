using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Animações")]
    public Animator animatorCorpo;
    public Animator animatorPerna;

    [Header("Movimentação")]
    public float velocidadeNormal = 5f;
    public float velocidadeAgachado = 2f;

    [Header("Referências")]
    public Transform cuboSuperior;
    public Transform cuboInferior;
    public GameObject groundDetector;
    private LifeScript lifeScript;

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
    public Transform disparoPonta;
    private Vector3 posicaoRelativaDisparoPonta;

    [Header("Granada")]
    public GameObject prefabGranada;
    public float forcaLancamentoGranada = 5f;
    public float cooldownGranada = 1f;
    public int granadasRestantes = 3;
    private float proximoLancamentoGranada;

    [Header("Especial")]
    public bool especial = false;
    public float raioDoEspecial = 5f;

    private Rigidbody2D rb;
    private bool estaAgachado = false;
    private bool podePular = false;
    private bool olhandoParaDireita = true;

    private Vector3 posicaoOriginalCuboSuperior;
    private Vector3 posicaoAgachadoCuboSuperior;
    private int contatoComChao = 0;

    // Hashes de animação
    private int movendoHash = Animator.StringToHash("Movendo");
    private int saltandoHash = Animator.StringToHash("Saltando");
    private int movendoCimaHash = Animator.StringToHash("MovendoCima");
    private int atirandoHash = Animator.StringToHash("Atirando");
    private int atirandoCimaHash = Animator.StringToHash("AtirandoCima");
    private int olhandoCimaHash = Animator.StringToHash("OlhandoCima");
    private int granadaHash = Animator.StringToHash("Granada");
    private int agacharHash = Animator.StringToHash("Agachado");
    private int andandoAgachadoHash = Animator.StringToHash("AndandoAgachado");

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lifeScript = GetComponent<LifeScript>();

        // Calcula posições do cubo superior para agachar
        posicaoOriginalCuboSuperior = cuboSuperior.localPosition;
        posicaoAgachadoCuboSuperior = posicaoOriginalCuboSuperior - new Vector3(0f, 0.2f, 0f);

        // Calcula posição relativa do DisparoPonta ao cuboSuperior
        if (disparoPonta != null)
            posicaoRelativaDisparoPonta = disparoPonta.localPosition - cuboSuperior.localPosition;
    }

    void Update()
    {
        Mover();
        Agachar();
        Pular();

        AtualizarAnimacoes();

        if (Keyboard.current.oKey.wasPressedThisFrame)
            Atirar();

        if (Keyboard.current.qKey.wasPressedThisFrame)
            LancarGranada();

        if (Keyboard.current.fKey.wasPressedThisFrame && especial)
        {
            LancarEspecial(transform.position, raioDoEspecial);
            especial = false;
        }

        if (lifeScript != null && lifeScript.GetVidaAtual() <= 0)
            SceneManager.LoadScene("Morte");
    }

    #region Movimentação
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

        // Ajusta escala do sprite
        Vector3 escala = transform.localScale;
        escala.x = olhandoParaDireita ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);
        transform.localScale = escala;

        // Atualiza animações
        if (moveX != 0)
        {
            animatorPerna.SetBool(andandoAgachadoHash, estaAgachado);
            animatorPerna.SetBool(movendoHash, !estaAgachado);
            animatorCorpo.SetBool(movendoCimaHash, true);
        }
        else
        {
            animatorPerna.SetBool(andandoAgachadoHash, false);
            animatorPerna.SetBool(movendoHash, false);
            animatorCorpo.SetBool(movendoCimaHash, false);
        }
    }

    void Agachar()
    {
        bool agachar = Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed;
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

        // Atualiza posição do DisparoPonta
        if (disparoPonta != null)
            disparoPonta.localPosition = cuboSuperior.localPosition + posicaoRelativaDisparoPonta;
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
    #endregion

    #region Ataques
    void Atirar()
    {
        if (prefabTiro == null || pontoDisparo == null) return;

        Vector2 direcao = olhandoParaDireita ? Vector2.right : Vector2.left;
        Quaternion rotacao = Quaternion.identity;
        Vector3 posicaoDisparo = pontoDisparo.position;

        if (Keyboard.current.wKey.isPressed)
        {
            direcao = Vector2.up;
            posicaoDisparo = cuboSuperior.position + new Vector3(0f, 0.7f, 0f);
            rotacao = Quaternion.Euler(0, 0, 90);
        }
        else if (!olhandoParaDireita)
        {
            rotacao = Quaternion.Euler(0, 0, 180);
        }

        GameObject tiro = Instantiate(prefabTiro, posicaoDisparo, rotacao);
        tiro.tag = "Danger";

        Rigidbody2D rbTiro = tiro.GetComponent<Rigidbody2D>();
        if (rbTiro != null)
            rbTiro.linearVelocity = direcao * velocidadeTiro;
    }

    void LancarGranada()
    {
        // só verifica cooldown e granadas
        if (Time.time < proximoLancamentoGranada || granadasRestantes <= 0) return;

        granadasRestantes--;
        proximoLancamentoGranada = Time.time + cooldownGranada;

        // apenas toca a animação do corpo
        animatorCorpo.SetTrigger(granadaHash);
    }

    // este método será chamado via Animation Event
    public void SpawnGranada()
    {
        if (prefabGranada == null || pontoDisparo == null) return;

        GameObject granada = Instantiate(prefabGranada, pontoDisparo.position, Quaternion.identity);
        Rigidbody2D rbGranada = granada.GetComponent<Rigidbody2D>();
        if (rbGranada != null)
        {
            Vector2 forca = new Vector2(olhandoParaDireita ? 1 : -1, 1).normalized * forcaLancamentoGranada;
            rbGranada.AddForce(forca, ForceMode2D.Impulse);
        }
    }

    public void LancarEspecial(Vector2 centro, float raio)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(centro, raio);
        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Enemy"))
            {
                ChefeHiller boss = col.GetComponentInParent<ChefeHiller>();
                if (boss != null)
                {
                    Debug.Log($"Especial atingiu o Boss: {boss.gameObject.name}");
                    boss.ReceberDanoEspecial();
                    return;
                }
                Destroy(col.gameObject); // inimigo comum
            }
        }
    }
    #endregion

    #region Escudo
    public void AtivarEscudo(float duracao)
    {
        if (lifeScript == null) return;

        lifeScript.ignorarDano = true;
        StartCoroutine(DesativarEscudo(duracao));
    }

    private IEnumerator DesativarEscudo(float duracao)
    {
        yield return new WaitForSeconds(duracao);
        if (lifeScript != null) lifeScript.ignorarDano = false;
    }
    #endregion

    #region Detecção de chão
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            contatoComChao++;
            podePular = contatoComChao > 0;
            animatorPerna.SetBool(saltandoHash, false);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            contatoComChao--;
            podePular = contatoComChao > 0;
        }
    }
    #endregion

    void AtualizarAnimacoes()
    {
        animatorCorpo.SetBool(olhandoCimaHash, Keyboard.current.wKey.isPressed);
        animatorCorpo.SetBool(atirandoHash, Keyboard.current.oKey.wasPressedThisFrame);
        animatorCorpo.SetBool(atirandoCimaHash, Keyboard.current.eKey.isPressed && Keyboard.current.wKey.isPressed);
    }
}
