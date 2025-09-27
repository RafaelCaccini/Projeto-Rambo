using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro; // necessário pra usar textos do TextMeshPro

public class AnimacaoDeIntroducao : MonoBehaviour
{
    [Header("Configurações de Tempo")]
    public float duracaoTotalAnimacao = 3f; // tempo total da animação
    public float duracaoFade = 1f; // tempo que leva pra aparecer (fade in)

    [Header("Botão (Objeto 2)")]
    public Image botaoImage; // imagem do botão
    public TextMeshProUGUI botaoTexto; // texto do botão

    [Header("Imagem 3")]
    public Image objeto3Image; // outra imagem que vai aparecer

    void Start()
    {
        // começa tudo transparente
        DefinirTransparenciaImage(botaoImage, 0f);
        DefinirTransparenciaTexto(botaoTexto, 0f);
        DefinirTransparenciaImage(objeto3Image, 0f);

        // começa a animação
        StartCoroutine(GerenciarAparecimento());
    }

    IEnumerator GerenciarAparecimento()
    {
        // espera metade do tempo antes de começar o fade
        float tempoEspera = duracaoTotalAnimacao / 2f;
        yield return new WaitForSeconds(tempoEspera);

        // começa o fade in do botão e da imagem 3
        StartCoroutine(FazerFadeInImage(botaoImage));
        StartCoroutine(FazerFadeInTexto(botaoTexto));
        StartCoroutine(FazerFadeInImage(objeto3Image));
    }

    IEnumerator FazerFadeInImage(Image imagem)
    {
        if (imagem == null) yield break; // se não tiver imagem, sai

        float tempoDecorrido = 0f;

        while (tempoDecorrido < duracaoFade)
        {
            tempoDecorrido += Time.deltaTime;
            float alpha = Mathf.Clamp01(tempoDecorrido / duracaoFade);

            DefinirTransparenciaImage(imagem, alpha); // atualiza transparência

            yield return null;
        }

        DefinirTransparenciaImage(imagem, 1f); // garante que fique 100% visível
    }

    IEnumerator FazerFadeInTexto(TextMeshProUGUI texto)
    {
        if (texto == null) yield break; // se não tiver texto, sai

        float tempoDecorrido = 0f;

        while (tempoDecorrido < duracaoFade)
        {
            tempoDecorrido += Time.deltaTime;
            float alpha = Mathf.Clamp01(tempoDecorrido / duracaoFade);

            DefinirTransparenciaTexto(texto, alpha); // atualiza transparência

            yield return null;
        }

        DefinirTransparenciaTexto(texto, 1f); // garante que fique 100% visível
    }

    // muda a transparência de uma imagem
    void DefinirTransparenciaImage(Image imagem, float alpha)
    {
        if (imagem != null)
        {
            Color cor = imagem.color;
            cor.a = alpha;
            imagem.color = cor;
        }
    }

    // muda a transparência de um texto
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
