using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // necessário pra trocar de cena

public class LifeScript : MonoBehaviour
{
    [Header("Configuração de Vida")]
    public int vidaMaxima = 100; // valor ajustável no inspetor
    [SerializeField] private int vidaAtual; // visível no inspector em tempo real

    [Header("Configuração de Dano")]
    public int danoPorToque = 10; // valor ajustável no inspetor

    [Header("Feedback de Dano")]
    public Color corDano = Color.red;
    public float tempoPiscar = 0.1f;

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
        if (col.collider.CompareTag("Danger"))
        {
            TomarDano(danoPorToque);
        }
    }

    public void TomarDano(int dano)
    {
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
        SceneManager.LoadScene("Morte");
    }

    // Método público para outros scripts consultarem a vida atual
    public int GetVidaAtual()
    {
        return vidaAtual;
    }
}
