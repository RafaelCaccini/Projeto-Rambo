using System.Collections;
using UnityEngine;

public class LifeScript : MonoBehaviour
{
    [Header("Configuração de Vida")]
    public int vidaMaxima = 100;
    [SerializeField] public int vidaAtual;

    [Header("Dano por Tag")]
    public int danoDanger = 10;
    public int danoDanger2 = 15;
    public int danoDanger3 = 20;
    public int danoDanger4 = 25;
    public bool ignorarDano = false;

    [Header("Feedback de Dano")]
    public Color corDano = Color.red;
    public float tempoPiscar = 0.1f;

    [Tooltip("Item que pode dropar.")]
    public GameObject itemParaDropar;
    [Range(0, 100)]
    public float chanceDeDrop = 3f;

    [Header("Referências")]
    public EspecialScript especialObj;
    public Animator animadorCima;      // Player
    public Animator animadorInimigo;   // Enemy/Boss

    private SpriteRenderer[] renderers;
    private Color[] coresOriginais;

    private bool morto = false;

    public delegate void MorteDelegate();
    public event MorteDelegate OnMorte;

    void Start()
    {
        vidaAtual = vidaMaxima;
        renderers = GetComponentsInChildren<SpriteRenderer>();
        coresOriginais = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            coresOriginais[i] = renderers[i].color;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!enabled) return;
        if (morto) return;

        int dano = 0;
        switch (col.collider.tag)
        {
            case "Danger": dano = danoDanger; break;
            case "Danger2": dano = danoDanger2; break;
            case "Danger3": dano = danoDanger3; break;
            case "Danger4": dano = danoDanger4; break;
        }

        if (dano > 0) TomarDano(dano);
    }

    public void TomarDano(int dano)
    {
        if (!enabled) return;
        if (ignorarDano || morto) return;

        vidaAtual -= dano;
        StartCoroutine(PiscarVermelho());

        if (vidaAtual <= 0)
        {
            vidaAtual = 0;
            Morrer();
        }
    }

    IEnumerator PiscarVermelho()
    {
        if (renderers == null) yield break;

        foreach (var r in renderers)
        {
            if (especialObj != null && r.gameObject == especialObj.gameObject) continue;
            r.color = corDano;
        }

        yield return new WaitForSeconds(tempoPiscar);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (especialObj != null && renderers[i].gameObject == especialObj.gameObject) continue;
            renderers[i].color = coresOriginais[i];
        }
    }

    void Morrer()
    {
        if (morto) return;
        morto = true;
        OnMorte?.Invoke();

        // Player
        if (CompareTag("Player"))
        {
            if (animadorCima != null)
                animadorCima.SetTrigger("Morrendo");

            var player = GetComponent<PlayerController>();
            if (player != null && player.animatorPerna != null)
            {
                var renderersPerna = player.animatorPerna.GetComponentsInChildren<SpriteRenderer>();
                foreach (var r in renderersPerna)
                {
                    Color c = r.color;
                    c.a = 0f;
                    r.color = c;
                }
            }

            // Travar movimento apenas pelos eixos do Rigidbody2D
            TravarMovimento();

            return; // cena será carregada via Animation Event
        }

        // Enemy ou Boss
        if (CompareTag("Enemy") || CompareTag("Boss"))
        {
            if (animadorInimigo != null)
                animadorInimigo.SetTrigger("InimigoMorrendo");

            TravarMovimento();
            TentarDroparItem();

            return;
        }

        // Para outros objetos genéricos
        TravarMovimento();
        Destroy(gameObject);
    }

    public void ForcarMorte()
    {
        if (morto) return;
        vidaAtual = 0;
        Morrer();
    }

    public int GetVidaAtual() => vidaAtual;
    public void DesativarEscudo() => ignorarDano = false;

    // ===================== Dano por tempo =====================
    public void IniciarDanoPorTempo(int danoPorTick, float duracaoTotal, float intervalo)
    {
        if (!enabled || morto) return;

        StartCoroutine(AplicarDanoPorTempo(danoPorTick, duracaoTotal, intervalo));
    }

    private IEnumerator AplicarDanoPorTempo(int danoPorTick, float duracaoTotal, float intervalo)
    {
        float tempoDecorrido = 0f;

        while (tempoDecorrido < duracaoTotal && !morto)
        {
            if (!enabled) yield break;

            TomarDano(danoPorTick);
            yield return new WaitForSeconds(intervalo);
            tempoDecorrido += intervalo;
        }
    }

    // ===================== Drop =====================
    public void TentarDroparItem()
    {
        float numeroAleatorio = Random.Range(0f, 100f);
        if (numeroAleatorio <= chanceDeDrop && itemParaDropar != null)
        {
            Instantiate(itemParaDropar, transform.position, Quaternion.identity);
        }
    }

    // NOVO MÉTODO PARA TRAVAR O MOVIMENTO
    private void TravarMovimento()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        }

        // Para inimigos comuns, desativa o script de movimento
        if (CompareTag("Enemy"))
        {
            Espartano espartano = GetComponent<Espartano>();
            if (espartano != null)
            {
                espartano.enabled = false;
            }
        }
        // Para Boss, NÃO desativa nada além do Rigidbody2D!
    }


    public void DestruirPosMorte()
    {
        Destroy(gameObject);
    }
}
