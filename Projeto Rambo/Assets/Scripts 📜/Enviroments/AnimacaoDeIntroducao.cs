using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro; // **IMPORTANTE: Necessário para usar TextMeshProUGUI**

public class AnimacaoDeIntroducao : MonoBehaviour
{
    [Header("Configurações de Tempo")]
    [Tooltip("Tempo total da animação principal (Objeto 1), em segundos.")]
    public float duracaoTotalAnimacao = 3f;

    [Tooltip("Duração do efeito de Fade In (de 0% a 100% de opacidade).")]
    public float duracaoFade = 1f; // Tempo que leva para os objetos aparecerem

    [Header("Botão (Objeto 2)")]
    [Tooltip("O componente Image do Botão.")]
    public Image botaoImage;
    [Tooltip("O componente TextMeshProUGUI do texto do Botão.")]
    public TextMeshProUGUI botaoTexto;

    [Header("Imagem 3")]
    [Tooltip("O componente Image da Imagem 3.")]
    public Image objeto3Image;


    void Start()
    {
        // Garante que todos os elementos visuais comecem totalmente transparentes
        DefinirTransparenciaImage(botaoImage, 0f);
        DefinirTransparenciaTexto(botaoTexto, 0f);
        DefinirTransparenciaImage(objeto3Image, 0f);

        // Inicia a corrotina principal
        StartCoroutine(GerenciarAparecimento());
    }

    IEnumerator GerenciarAparecimento()
    {
        // 1. ESPERA: Espera metade do tempo total da animação principal
        float tempoEspera = duracaoTotalAnimacao / 2f;
        yield return new WaitForSeconds(tempoEspera);

        // 2. FADE IN: Inicia o processo de aparecimento (Degradê)

        // Inicia o Fade In do Botão (Fundo e Texto)
        StartCoroutine(FazerFadeInImage(botaoImage));
        StartCoroutine(FazerFadeInTexto(botaoTexto));

        // Inicia o Fade In da Imagem 3
        StartCoroutine(FazerFadeInImage(objeto3Image));
    }

    IEnumerator FazerFadeInImage(Image imagem)
    {
        if (imagem == null) yield break;

        float tempoDecorrido = 0f;

        while (tempoDecorrido < duracaoFade)
        {
            tempoDecorrido += Time.deltaTime;
            float alpha = Mathf.Clamp01(tempoDecorrido / duracaoFade);

            DefinirTransparenciaImage(imagem, alpha);

            yield return null;
        }

        DefinirTransparenciaImage(imagem, 1f);
    }

    IEnumerator FazerFadeInTexto(TextMeshProUGUI texto)
    {
        if (texto == null) yield break;

        float tempoDecorrido = 0f;

        while (tempoDecorrido < duracaoFade)
        {
            tempoDecorrido += Time.deltaTime;
            float alpha = Mathf.Clamp01(tempoDecorrido / duracaoFade);

            DefinirTransparenciaTexto(texto, alpha);

            yield return null;
        }

        DefinirTransparenciaTexto(texto, 1f);
    }

    // Função auxiliar para mudar a transparência de um componente Image
    void DefinirTransparenciaImage(Image imagem, float alpha)
    {
        if (imagem != null)
        {
            Color cor = imagem.color;
            cor.a = alpha;
            imagem.color = cor;
        }
    }

    // Função auxiliar para mudar a transparência de um componente TextMeshPro
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