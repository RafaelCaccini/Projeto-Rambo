using System.Collections;
using UnityEngine;

public class LifeScript : MonoBehaviour
{
    [Header("Configura��o de Vida")]
    public int vidaMaxima = 100; // valor ajust�vel no inspetor
    private int vidaAtual;

    [Header("Configura��o de Dano")]
    public int danoPorToque = 10; // valor ajust�vel no inspetor

    [Header("Feedback de Dano")]
    public Color corDano = Color.red;
    public float tempoPiscar = 0.1f;

    private SpriteRenderer[] renderers;
    private Color[] coresOriginais;

    void Start()
    {
        vidaAtual = vidaMaxima;

        // Pega todos os SpriteRenderers do objeto e filhos pra toidos morrer
        renderers = GetComponentsInChildren<SpriteRenderer>();

        // Guarda as cores originais para ele n ficar vermelho semrpe
        coresOriginais = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            coresOriginais[i] = renderers[i].color;
        }
    }

    void OnCollisionEnter2D(Collision2D col) //Detecta a colis�o com outra caixa de colis�o. Mesmo sem ser isTrigger.
    {
        if (col.collider.CompareTag("Danger")) // aplica apenas se o objeto tiver a tag Danger.
        {
            TomarDano(danoPorToque);
        }
    }

    public void TomarDano(int dano) // metodo para subtrair a vida por toquer no objeto perigoso.
    {
        vidaAtual -= dano;
        StartCoroutine(PiscarVermelho());

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    IEnumerator PiscarVermelho() //Fun��o "Pausavel" IEnumerator(serve para criar uma fun��o que s� inicia, pausa., e depois volta com outra condi��o, ou, depois de um timer), no caso, a condi��o � o contato.
    {
        // Fica vermelho
        foreach (var r in renderers)
        {
            r.color = corDano;
        }

        yield return new WaitForSeconds(tempoPiscar);

        // Volta pra cor original
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].color = coresOriginais[i];
        }
    }

    void Morrer() // mata o jogador caso a vida seja = a 0, ou menor
    {
        Destroy(gameObject);
    }
}
