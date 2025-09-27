using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro; // necess�rio pra usar textos do TextMeshPro

public class AnimacaoDeIntroducao : MonoBehaviour
{
    [Header("Configura��es de Tempo")]
    public float duracaoTotalAnimacao = 3f; // tempo total da anima��o
    public float duracaoFade = 1f; // tempo que leva pra aparecer (fade in)

    [Header("Bot�o (Objeto 2)")]
    public Image botaoImage; // imagem do bot�o
    public TextMeshProUGUI botaoTexto; // texto do bot�o

    [Header("Imagem 3")]
    public Image objeto3Image; // outra imagem que vai aparecer

    void Start()
    {
        // come�a tudo transparente
        DefinirTransparenciaImage(botaoImage, 0f);
        DefinirTransparenciaTexto(botaoTexto, 0f);
        DefinirTransparenciaImage(objeto3Image, 0f);

        // come�a a anima��o
        StartCoroutine(GerenciarAparecimento());
    }

    IEnumerator GerenciarAparecimento()
    {
        // espera metade do tempo antes de come�ar o fade
        float tempoEspera = duracaoTotalAnimacao / 2f;
        yield return new WaitForSeconds(tempoEspera);

        // come�a o fade in do bot�o e da imagem 3
        StartCoroutine(FazerFadeInImage(botaoImage));
        StartCoroutine(FazerFadeInTexto(botaoTexto));
        StartCoroutine(FazerFadeInImage(objeto3Image));
    }

    IEnumerator FazerFadeInImage(Image imagem)
    {
        if (imagem == null) yield break; // se n�o tiver imagem, sai

        float tempoDecorrido = 0f;

        while (tempoDecorrido < duracaoFade)
        {
            tempoDecorrido += Time.deltaTime;
            float alpha = Mathf.Clamp01(tempoDecorrido / duracaoFade);

            DefinirTransparenciaImage(imagem, alpha); // atualiza transpar�ncia

            yield return null;
        }

        DefinirTransparenciaImage(imagem, 1f); // garante que fique 100% vis�vel
    }

    IEnumerator FazerFadeInTexto(TextMeshProUGUI texto)
    {
        if (texto == null) yield break; // se n�o tiver texto, sai

        float tempoDecorrido = 0f;

        while (tempoDecorrido < duracaoFade)
        {
            tempoDecorrido += Time.deltaTime;
            float alpha = Mathf.Clamp01(tempoDecorrido / duracaoFade);

            DefinirTransparenciaTexto(texto, alpha); // atualiza transpar�ncia

            yield return null;
        }

        DefinirTransparenciaTexto(texto, 1f); // garante que fique 100% vis�vel
    }

    // muda a transpar�ncia de uma imagem
    void DefinirTransparenciaImage(Image imagem, float alpha)
    {
        if (imagem != null)
        {
            Color cor = imagem.color;
            cor.a = alpha;
            imagem.color = cor;
        }
    }

    // muda a transpar�ncia de um texto
    void DefinirTransparenciaTexto(TextMeshProUGUI texto, float alpha)
    {
        if (texto != null)
        {
            Color cor = texto.color;
            cor.a = alpha;
            texto.color = cor;
        }
    }
}
