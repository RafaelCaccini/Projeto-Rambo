using System.Collections;
using UnityEngine;

public class LifeScript : MonoBehaviour
{
    [Header("Configuração de Vida")]
    public int vidaMaxima = 100; // valor ajustável no inspetor
    private int vidaAtual;

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

        // Pega todos os SpriteRenderers do objeto e filhos pra toidos morrer
        renderers = GetComponentsInChildren<SpriteRenderer>();

        // Guarda as cores originais para ele n ficar vermelho semrpe
        coresOriginais = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            coresOriginais[i] = renderers[i].color;
        }
    }

    void OnCollisionEnter2D(Collision2D col) //Detecta a colisão com outra caixa de colisão. Mesmo sem ser isTrigger.
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

    IEnumerator PiscarVermelho() //Função "Pausavel" IEnumerator(serve para criar uma função que só inicia, pausa., e depois volta com outra condição, ou, depois de um timer), no caso, a condição é o contato.
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
