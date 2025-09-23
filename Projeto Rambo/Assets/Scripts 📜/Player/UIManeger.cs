using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // === Referências da UI ===
    [Header("UI do Jogador")]
    public Slider healthBar;
    public TextMeshProUGUI grenadeCountText;
    public GameObject specialActiveImage;

    [Header("UI do Escudo")]
    public GameObject shieldImage; // O objeto que é a imagem do escudo

    // === Referência do Jogador ===
    private PlayerController playerController;
    private LifeScript lifeScript;

    void Start()
    {
        // Encontra o objeto do jogador na cena (certifique-se que ele tem a tag "Player")
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Pega as referências dos scripts do jogador
            playerController = player.GetComponent<PlayerController>();
            lifeScript = player.GetComponent<LifeScript>();

            // Configura o valor máximo da barra de vida
            if (healthBar != null && lifeScript != null)
            {
                healthBar.maxValue = lifeScript.vidaMaxima;
            }

            // Garante que a imagem do escudo comece desativada
            if (shieldImage != null)
            {
                shieldImage.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("Jogador não encontrado. Certifique-se de que o objeto do jogador tem a tag 'Player'.");
        }
    }

    void Update()
    {
        // Atualiza a UI a cada frame
        UpdateHealthBar();
        UpdateGrenadeCount();
        UpdateSpecialStatus();
        UpdateShieldStatus(); // Nova função para o escudo
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null && lifeScript != null)
        {
            // O valor da barra de vida é igual à vida atual do jogador
            healthBar.value = lifeScript.vidaAtual;
        }
    }

    private void UpdateGrenadeCount()
    {
        if (grenadeCountText != null && playerController != null)
        {
            // Atualiza o texto com a quantidade de granadas
            grenadeCountText.text = playerController.granadasRestantes.ToString();
        }
    }

    private void UpdateSpecialStatus()
    {
        if (specialActiveImage != null && playerController != null)
        {
            // Ativa ou desativa a imagem do especial com base na variável booleana 'especial'
            specialActiveImage.SetActive(playerController.especial);
        }
    }

    // ---
    // NOVO MÉTODO PARA O ESCUDO
    // ---
    private void UpdateShieldStatus()
    {
        if (shieldImage != null && lifeScript != null)
        {
            // Ativa ou desativa a imagem do escudo com base na variável 'ignorarDano'
            shieldImage.SetActive(lifeScript.ignorarDano);
        }
    }
}