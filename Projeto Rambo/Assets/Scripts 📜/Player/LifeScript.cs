using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // NÃO usamos para carregar a cena do Player aqui (Animation Event fará isso)

public class LifeScript : MonoBehaviour
{
    [Header("Configuração de Vida")]
    public int vidaMaxima = 100; // vida máxima ajustável
    [SerializeField] public int vidaAtual; // vida atual visível no inspector

    [Header("Dano por Tag")]
    public int danoDanger = 10;
    public int danoDanger2 = 15;
    public int danoDanger3 = 20;
    public int danoDanger4 = 25;
    public bool ignorarDano = false; // se true, ignora todo dano

    [Header("Feedback de Dano")]
    public Color corDano = Color.red; // cor que pisca ao tomar dano
    public float tempoPiscar = 0.1f;  // tempo do piscar

    [Tooltip("Item que pode dropar.")]
    public GameObject itemParaDropar;
    [Range(0, 100)]
    public float chanceDeDrop = 3f; // porcentagem de drop

    [Header("Carregar Cena ao Morrer")]
    // OBS: para o Player, a cena deve ser carregada via Animation Event (no torso). 
    // Este campo é mantido para compatibilidade com outros usos.
    public bool carregarCenaAoMorrer = false;
    public string nomeCenaMorte;

    [Header("Referências")]
    public EspecialScript especialObj; // referência ao objeto especial (se existir)
    public Animator animadorCima; // Animator da parte de cima — arraste no inspector (usado para trigger "Morrendo")

    private SpriteRenderer[] renderers; // pra piscar
    private Color[] coresOriginais;     // guarda as cores originais

    private bool morto = false;

    // Evento opcional caso outros scripts queiram reagir ao morrer
    public delegate void MorteDelegate();
    public event MorteDelegate OnMorte;

    void Start()
    {
        vidaAtual = vidaMaxima;

        // pega todos os sprites do objeto e filhos
        renderers = GetComponentsInChildren<SpriteRenderer>();
        coresOriginais = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
            coresOriginais[i] = renderers[i].color;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (morto) return;

        int dano = 0;

        // define o dano baseado na tag
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
            // ignora o objeto especial (se quiser que ele não pisque)
            if (especialObj != null && r.gameObject == especialObj.gameObject)
                continue;

            r.color = corDano;
        }

        yield return new WaitForSeconds(tempoPiscar);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (especialObj != null && renderers[i].gameObject == especialObj.gameObject)
                continue;

            renderers[i].color = coresOriginais[i];
        }
    }

    // Lógica central de morte (Player vs outros)
    void Morrer()
    {
        if (morto) return;
        morto = true;
        OnMorte?.Invoke();

        if (CompareTag("Player"))
        {
            // Dispara trigger de morte no animator de cima
            if (animadorCima != null)
                animadorCima.SetTrigger("Morrendo");

            // Deixa parte de baixo (pernas) totalmente invisível
            var player = GetComponent<PlayerController>();
            if (player != null && player.animatorPerna != null)
            {
                var renderers = player.animatorPerna.GetComponentsInChildren<SpriteRenderer>();
                foreach (var r in renderers)
                {
                    Color c = r.color;
                    c.a = 0f; // invisível total
                    r.color = c;
                }
            }

            // Desativa física e movimento
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.simulated = false;
            }

            return; // cena será carregada pelo Animation Event
        }

        if (CompareTag("Enemy"))
        {
            TentarDroparItem();
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }


    // Força morte programaticamente (útil para debug/cheats)
    public void ForcarMorte()
    {
        if (morto) return;
        vidaAtual = 0;
        Morrer();
    }

    // Retorna a vida atual pra outros scripts
    public int GetVidaAtual() => vidaAtual;

    // Reativa o recebimento de dano
    public void DesativarEscudo() => ignorarDano = false;

    // ===================== Dano por tempo (DOT) =====================
    // Inicia dano por tempo: danoPorTick em intervalos, por duracaoTotal segundos.
    public void IniciarDanoPorTempo(int danoPorTick, float duracaoTotal, float intervalo)
    {
        if (morto) return;
        StartCoroutine(AplicarDanoPorTempo(danoPorTick, duracaoTotal, intervalo));
    }

    private IEnumerator AplicarDanoPorTempo(int danoPorTick, float duracaoTotal, float intervalo)
    {
        float tempoDecorrido = 0f;

        while (tempoDecorrido < duracaoTotal && !morto)
        {
            TomarDano(danoPorTick);
            // Debug.Log($"Dano por tempo: -{danoPorTick}. Restante: {duracaoTotal - tempoDecorrido}s");

            yield return new WaitForSeconds(intervalo);
            tempoDecorrido += intervalo;
        }

        // Debug.Log("Dano por tempo finalizado.");
    }


    public void TentarDroparItem()
    {
        float numeroAleatorio = Random.Range(0f, 100f);

        if (numeroAleatorio <= chanceDeDrop)
        {
            if (itemParaDropar != null)
            {
                Instantiate(itemParaDropar, transform.position, Quaternion.identity);
                // Debug.Log("Item dropado!");
            }
            else
            {
                Debug.LogWarning("Item de drop não definido!");
            }
        }
        else
        {
            // Debug.Log($"Nada dropado. Chance era {chanceDeDrop}%.");
        }
    }
}
