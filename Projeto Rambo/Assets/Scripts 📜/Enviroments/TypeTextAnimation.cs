using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro; // necess�rio pra usar textos do TextMeshPro

public class TypeTextAnimation : MonoBehaviour
{
    [Header("Configura��es de Texto Principal")]
    public float typeDelay = 0.05f; //tempo de intervalo entre o aparecimento de cada letra
    public TextMeshProUGUI textObject;
    public string fullText; //texto completo a ser exibido
    public float startDelay = 1f; //tempo de espera antes de come�ar a anima��o

    [Header("Configura��es de Texto Secund�rio")]
    public bool showSecondaryText = false; // Ativa ou desativa o texto secund�rio
    public string secondaryText; // O texto secund�rio
    public float delayBetweenTexts = 1.0f; // Tempo de espera entre o fim do primeiro e o in�cio do segundo texto

    void Start()
    {
        // Inicia a corrotina que gerencia toda a sequ�ncia de texto
        StartCoroutine(TypeTextSequence());
    }


    /// Corrotina principal que gerencia a exibi��o do texto.
  
    IEnumerator TypeTextSequence()
    {
        // Espera o tempo de delay inicial
        yield return new WaitForSeconds(startDelay);

        // Exibe o primeiro texto
        yield return StartCoroutine(AnimateText(fullText));

        // Se o texto secund�rio estiver habilitado
        if (showSecondaryText)
        {
            // Espera o tempo de delay entre os textos
            yield return new WaitForSeconds(delayBetweenTexts);

            // Exibe o texto secund�rio no lugar do primeiro
            yield return StartCoroutine(AnimateText(secondaryText));
        }
    }

 
    /// Corrotina que faz a anima��o de digita��o de um texto.
   
 
    IEnumerator AnimateText(string textToAnimate)
    {
        // Define o texto completo no objeto e esconde tudo
        textObject.text = textToAnimate;
        textObject.maxVisibleCharacters = 0;

        // Anima��o: mostra um caractere de cada vez
        for (int i = 0; i <= textToAnimate.Length; i++)
        {
            textObject.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typeDelay);
        }
    }
}