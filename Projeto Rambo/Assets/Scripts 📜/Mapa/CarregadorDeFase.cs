using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Novo Input System

public class CarregadorDeFase : MonoBehaviour
{
    [Header("Configura��o da Fase")]
    [Tooltip("Nome da cena para carregar (igual ao nome em Build Settings)")]
    public string nomeDaFase; // Qual cena carregar

    [Header("Configura��o de Visibilidade")]
    [Tooltip("Arraste aqui o objeto que deve se tornar vis�vel.")]
    public SpriteRenderer objetoVisivel; // O bagulho que vai aparecer

    private bool playerDentro = false; // Player ta na area?

    void Start()
    {
        // Esse bagulho precisa come�ar invis�vel
        if (objetoVisivel != null)
        {
            Color cor = objetoVisivel.color;
            cor.a = 0f; // Seta o canal alfa (transpar�ncia) pra 0
            objetoVisivel.color = cor;
        }
    }

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

            // NOVO COMENT�RIO: A� SIM! Deixa o bagulho vis�vel
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
        // Detecta quando o player sai da �rea
        if (other.CompareTag("Player"))
        {
            playerDentro = false;
            Debug.Log("Player saiu da �rea do carregador.");

            // Deixa o bagulho invis�vel de novo.
            if (objetoVisivel != null)
            {
                Color cor = objetoVisivel.color;
                cor.a = 0f;
                objetoVisivel.color = cor;
            }
        }
    }
}