using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // necessário pra trocar de cena

public class LifeScript : MonoBehaviour
{
    [Header("Configuração de Vida")]
    public int vidaMaxima = 100; // valor ajustável no inspetor
    [SerializeField] public int vidaAtual; // visível no inspector em tempo real

    [Header("Configuração de Dano por Tag")]
    public int danoDanger = 10;   // "Danger" normal
    public int danoDanger2 = 15;  // "Danger2"
    public int danoDanger3 = 20;  // "Danger3"
    public int danoDanger4 = 25;  // "Danger4"
    public bool ignorarDano = false; 

    [Header("Feedback de Dano")]
    public Color corDano = Color.red;
    public float tempoPiscar = 0.1f;

    [Tooltip("O item que será dropado.")]
    public GameObject itemParaDropar;

    [Tooltip("A chance de drop, em porcentagem (ex: 3 para 3%).")]
    [Range(0, 100)]
    public float chanceDeDrop = 3f;

    private SpriteRenderer[] renderers;
    private Color[] coresOriginais;

    void Start()
    {
        vidaAtual = vidaMaxima;

        // Pega todos os SpriteRenderers do objeto e filhos
        renderers = GetComponentsInChildren<SpriteRenderer>();

        // Guarda as cores originais
        coresOriginais = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            coresOriginais[i] = renderers[i].color;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        int dano = 0;

        switch (col.collider.tag)
        {
            case "Danger":
                dano = danoDanger;
                break;
            case "Danger2":
                dano = danoDanger2;
                break;
            case "Danger3":
                dano = danoDanger3;
                break;
            case "Danger4":
                dano = danoDanger4;
                break;
        }

        if (dano > 0)
        {
            TomarDano(dano);
        }
    }

    public void TomarDano(int dano)
    {
        if (ignorarDano) return;

        vidaAtual -= dano;
        StartCoroutine(PiscarVermelho());

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    IEnumerator PiscarVermelho()
    {
        foreach (var r in renderers)
        {
            r.color = corDano;
        }

        yield return new WaitForSeconds(tempoPiscar);

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].color = coresOriginais[i];
        }
    }

    void Morrer()
    {
        // Carrega a cena "Morte"
        if (CompareTag("Player"))
        {
            SceneManager.LoadScene("Morte");
        }
        else if (CompareTag("Inimigo"))
        {
            TentarDroparItem();
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Método público para outros scripts consultarem a vida atual
    public int GetVidaAtual()
    {
        return vidaAtual;
    }
    public void DesativarEscudo()
    {
        ignorarDano = false;
    }
    public void TentarDroparItem()
    {
        float chance = Random.Range(0f, 100f);
        // Gera um número aleatório entre 0 e 100.
        float numeroAleatorio = Random.Range(0f, 100f);

        // Verifica se o número aleatório é menor ou igual à chance de drop.
        if (numeroAleatorio <= chanceDeDrop)
        {
            // Instancia o item na posição do inimigo.
            if (itemParaDropar != null)
            {
                Instantiate(itemParaDropar, transform.position, Quaternion.identity);
                Debug.Log("Item dropado com sucesso!");
            }
            else
            {
                Debug.LogWarning("O item a ser dropado não foi definido no Inspector!");
            }
        }
        else
        {
            Debug.Log("Nada dropado desta vez. Chance de drop era " + chanceDeDrop + "%.");
        }
    }
}


