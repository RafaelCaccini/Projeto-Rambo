using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Novo Input System

public class CarregadorDeFase : MonoBehaviour
{
    [Header("Configura��o da Fase")]
    [Tooltip("Nome da cena para carregar (igual ao nome em Build Settings)")]
    public string nomeDaFase; // Qual cena carregar

    private bool playerDentro = false; // Player est� na �rea?

    void Update()
    {
        // Se o player estiver na �rea e apertar E
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
        // Detecta quando o player entra na �rea
        if (other.CompareTag("Player"))
        {
            playerDentro = true;
            Debug.Log("Player entrou na �rea do carregador.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Detecta quando o player sai da �rea
        if (other.CompareTag("Player"))
        {
            playerDentro = false;
            Debug.Log("Player saiu da �rea do carregador.");
        }
    }
}
