using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // novo Input System

public class CarregadorDeFase : MonoBehaviour
{
    [Header("Configuração da Fase")]
    [Tooltip("Nome da cena para carregar (igual ao nome em Build Settings)")]
    public string nomeDaFase;

    private bool playerDentro = false;

    void Update()
    {
        // Se o player estiver dentro e apertar E
        if (playerDentro && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!string.IsNullOrEmpty(nomeDaFase))
            {
                SceneManager.LoadScene(nomeDaFase);
            }
            else
            {
                Debug.LogWarning("? Nenhum nome de cena definido no CarregadorDeFase!");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerDentro = true;
            Debug.Log("Player entrou na área do carregador.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerDentro = false;
            Debug.Log("Player saiu da área do carregador.");
        }
    }
}
