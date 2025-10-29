using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Sons")]
    public AudioSource audioSource;
    public AudioClip somPulo;
    public AudioClip somPasso;
    public AudioClip somTiro;
    public float intervaloSomPasso = 0.3f;
    private float proximoSomPasso = 0f;

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
    public EspecialScript especialObj;

    private Rigidbody2D rb;
    private bool estaAgachado = false;
    private bool podePular = false;
    private bool olhandoParaDireita = true;
    private bool morto = false;

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
    private int morrendoHash = Animator.StringToHash("Morrendo");

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lifeScript = GetComponent<LifeScript>();

        posicaoOriginalCuboSuperior = cuboSuperior.localPosition;
        posicaoAgachadoCuboSuperior = posicaoOriginalCuboSuperior - new Vector3(0f, 0.2f, 0f);

        if (disparoPonta != null)
            posicaoRelativaDisparoPonta = disparoPonta.localPosition - cuboSuperior.localPosition;
    }

    void Update()
    {
        if (morto) return;

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

            if (especialObj != null)
                especialObj.AtivarEspecial();
        }

        // Detecta morte
        if (lifeScript != null && lifeScript.GetVidaAtual() <= 0 && !morto)
            OnPlayerMorreu();
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

        Vector3 escala = transform.localScale;
        escala.x = olhandoParaDireita ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);
        transform.localScale = escala;

        if (moveX != 0)
        {
            animatorPerna.SetBool(andandoAgachadoHash, estaAgachado);
            animatorPerna.SetBool(movendoHash, !estaAgachado);
            animatorCorpo.SetBool(movendoCimaHash, true);

            // Som de passo
            if (contatoComChao > 0 && Time.time >= proximoSomPasso)
            {
                if (audioSource && somPasso)
                    audioSource.PlayOneShot(somPasso);
                proximoSomPasso = Time.time + intervaloSomPasso;
            }
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

            // Som de pulo
            if (audioSource && somPulo)
                audioSource.PlayOneShot(somPulo);
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

        // Som de tiro
        if (audioSource && somTiro)
            audioSource.PlayOneShot(somTiro);
    }

    void LancarGranada()
    {
        if (Time.time < proximoLancamentoGranada || granadasRestantes <= 0) return;

        granadasRestantes--;
        proximoLancamentoGranada = Time.time + cooldownGranada;
        animatorCorpo.SetTrigger(granadaHash);
    }

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
                Destroy(col.gameObject);
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

    #region Chão
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
        animatorCorpo.SetBool(atirandoCimaHash, Keyboard.current.oKey.isPressed && Keyboard.current.wKey.isPressed);
    }

    // === Chamado quando a vida chega a 0 ===
    public void OnPlayerMorreu()
    {
        morto = true;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        if (animatorCorpo != null)
            animatorCorpo.SetTrigger(morrendoHash);
    }

    // === Chamado pelo Animation Event ===
    public void FicarComPernaTransparente()
    {
        if (animatorPerna != null)
        {
            var renderers = animatorPerna.GetComponentsInChildren<SpriteRenderer>();
            foreach (var r in renderers)
            {
                Color c = r.color;
                c.a = 0.3f;
                r.color = c;
            }
        }
    }
}
