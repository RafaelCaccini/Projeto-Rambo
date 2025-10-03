using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // pra trocar de cena

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
    public bool carregarCenaAoMorrer = false; // se true, carrega cena ao morrer
    public string nomeCenaMorte; // nome da cena a carregar

    [Header("Referências")]
    public EspecialScript especialObj; // referência ao objeto especial

    private SpriteRenderer[] renderers; // pra piscar
    private Color[] coresOriginais;     // guarda as cores originais

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
        if (ignorarDano) return;

        vidaAtual -= dano;
        StartCoroutine(PiscarVermelho());

        if (vidaAtual <= 0) Morrer();
    }

    IEnumerator PiscarVermelho()
    {
        foreach (var r in renderers)
        {
            // ignora o objeto especial
            if (especialObj != null && r.gameObject == especialObj.gameObject)
                continue;

            r.color = corDano;
        }

        yield return new WaitForSeconds(tempoPiscar);

        for (int i = 0; i < renderers.Length; i++)
        {
            // ignora o objeto especial ao restaurar a cor
            if (especialObj != null && renderers[i].gameObject == especialObj.gameObject)
                continue;

            renderers[i].color = coresOriginais[i];
        }
    }

    void Morrer()
    {
        if (carregarCenaAoMorrer && !string.IsNullOrEmpty(nomeCenaMorte))
        {
            SceneManager.LoadScene(nomeCenaMorte);
            return;
        }

        if (CompareTag("Player"))
        {
            // player morre, vai pra cena de morte
            SceneManager.LoadScene("Morte");
        }
        else if (CompareTag("Enemy"))
        {
            // inimigo morre, tenta dropar item
            TentarDroparItem();
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // retorna a vida atual pra outros scripts
    public int GetVidaAtual() => vidaAtual;

    public void DesativarEscudo() => ignorarDano = false;

    // checa se dropa algum item
    public void TentarDroparItem()
    {
        float numeroAleatorio = Random.Range(0f, 100f);

        if (numeroAleatorio <= chanceDeDrop)
        {
            if (itemParaDropar != null)
            {
                Instantiate(itemParaDropar, transform.position, Quaternion.identity);
                Debug.Log("Item dropado!");
            }
            else Debug.LogWarning("Item de drop não definido!");
        }
        else
        {
            Debug.Log($"Nada dropado. Chance era {chanceDeDrop}%.");
        }
    }

    // aplica dano ao longo do tempo
    public void IniciarDanoPorTempo(int danoPorTick, float duracaoTotal, float intervalo)
    {
        StartCoroutine(AplicarDanoPorTempo(danoPorTick, duracaoTotal, intervalo));
    }

    IEnumerator AplicarDanoPorTempo(int danoPorTick, float duracaoTotal, float intervalo)
    {
        float tempoDecorrido = 0f;

        while (tempoDecorrido < duracaoTotal)
        {
            TomarDano(danoPorTick);
            Debug.Log($"Dano por tempo: -{danoPorTick}. Restante: {duracaoTotal - tempoDecorrido}s");

            yield return new WaitForSeconds(intervalo);
            tempoDecorrido += intervalo;
        }

        Debug.Log("Dano por tempo finalizado.");
    }
}
