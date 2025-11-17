using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheatActivatorProfessional : MonoBehaviour
{
    [Header("Referência ao LifeScript do jogador")]
    public LifeScript playerLife; // NÃO MEXER NO LifeScript.cs

    [Header("Códigos válidos")]
    public string[] cheatCodes = { "RAMBO", "36034" };

    [Tooltip("Duração da vida infinita em segundos")]
    public float cheatDurationSeconds = 300f; // 5 minutos

    private string inputBuffer = "";

    void OnEnable()
    {
        // Ativa callback de digitação
        Keyboard.current.onTextInput += OnTextInput;
    }

    void OnDisable()
    {
        if (Keyboard.current != null)
            Keyboard.current.onTextInput -= OnTextInput;
    }

    private void OnTextInput(char c)
    {
        inputBuffer += char.ToUpper(c);

        // Verifica códigos válidos
        foreach (var code in cheatCodes)
        {
            if (inputBuffer.EndsWith(code))
            {
                inputBuffer = "";
                StartCoroutine(ActivateLifeCheat());
                break;
            }
        }

        // Limita buffer
        if (inputBuffer.Length > 50)
            inputBuffer = "";
    }

    private IEnumerator ActivateLifeCheat()
    {
        if (playerLife == null)
        {
            Debug.LogWarning("[CheatActivator] LifeScript não atribuído. Cheat não ativado.");
            yield break;
        }

        playerLife.ignorarDano = true;
        playerLife.vidaAtual = playerLife.vidaMaxima;
        Debug.Log($"[CheatActivator] 🔥 Vida infinita ativada por {cheatDurationSeconds / 60f:F1} minutos!");

        float interval = 0.1f; // opcional, mantém a vida cheia constantemente
        float elapsed = 0f;
        while (elapsed < cheatDurationSeconds)
        {
            if (playerLife != null)
                playerLife.vidaAtual = playerLife.vidaMaxima;

            elapsed += interval;
            yield return new WaitForSeconds(interval);
        }

        if (playerLife != null)
            playerLife.ignorarDano = false;

        Debug.Log("[CheatActivator] ⏰ Vida infinita desativada!");
    }
}