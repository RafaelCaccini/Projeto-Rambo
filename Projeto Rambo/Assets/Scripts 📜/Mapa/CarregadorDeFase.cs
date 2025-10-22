using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Novo Input System

public class CarregadorDeFase : MonoBehaviour
{
    [Header("Configuração da Fase")]
    [Tooltip("Nome da cena para carregar (igual ao nome em Build Settings)")]
    public string nomeDaFase; // Qual cena carregar

    [Header("Configuração de Visibilidade")]
    [Tooltip("Arraste aqui o objeto que deve se tornar visível.")]
    public SpriteRenderer objetoVisivel; // O bagulho que vai aparecer

    private bool playerDentro = false; // Player ta na area?

    void Start()
    {
        // Esse bagulho precisa começar invisível
        if (objetoVisivel != null)
        {
            Color cor = objetoVisivel.color;
            cor.a = 0f; // Seta o canal alfa (transparência) pra 0
            objetoVisivel.color = cor;
        }
    }

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

            // NOVO COMENTÁRIO: AÍ SIM! Deixa o bagulho visível
            if (objetoVisivel != null)
            {
                Color cor = objetoVisivel.color;
                cor.a = 1f; // Seta o canal alfa pra 1
                objetoVisivel.color = cor;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Detecta quando o player sai da área
        if (other.CompareTag("Player"))
        {
            playerDentro = false;
            Debug.Log("Player saiu da área do carregador.");

            // Deixa o bagulho invisível de novo.
            if (objetoVisivel != null)
            {
                Color cor = objetoVisivel.color;
                cor.a = 0f;
                objetoVisivel.color = cor;
            }
        }
    }
}