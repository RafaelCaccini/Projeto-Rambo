using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Novo Input System

public class CarregadorDeFase : MonoBehaviour
{
    [Header("Configuração da Fase")]
    [Tooltip("Nome da cena para carregar (igual ao nome em Build Settings)")]
    public string nomeDaFase; // Qual cena carregar

    private bool playerDentro = false; // Player está na área?

    void Update()
    {
        // Se o player estiver na área e apertar E
        if (playerDentro && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!string.IsNullOrEmpty(nomeDaFase))
            {
                SceneManager.LoadScene(nomeDaFase); // Carrega a fase
            }
            else
            {
                Debug.LogWarning("? Nenhum nome de cena definido!");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Detecta quando o player entra na área
        if (other.CompareTag("Player"))
        {
            playerDentro = true;
            Debug.Log("Player entrou na área do carregador.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Detecta quando o player sai da área
        if (other.CompareTag("Player"))
        {
            playerDentro = false;
            Debug.Log("Player saiu da área do carregador.");
        }
    }
}
