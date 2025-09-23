using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // === Refer�ncias da UI ===
    [Header("UI do Jogador")]
    public Slider healthBar;
    public TextMeshProUGUI grenadeCountText;
    public GameObject specialActiveImage;

    [Header("UI do Escudo")]
    public GameObject shieldImage; // O objeto que � a imagem do escudo

    // === Refer�ncia do Jogador ===
    private PlayerController playerController;
    private LifeScript lifeScript;

    void Start()
    {
        // Encontra o objeto do jogador na cena (certifique-se que ele tem a tag "Player")
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Pega as refer�ncias dos scripts do jogador
            playerController = player.GetComponent<PlayerController>();
            lifeScript = player.GetComponent<LifeScript>();

            // Configura o valor m�ximo da barra de vida
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
            Debug.LogError("Jogador n�o encontrado. Certifique-se de que o objeto do jogador tem a tag 'Player'.");
        }
    }

    void Update()
    {
        // Atualiza a UI a cada frame
        UpdateHealthBar();
        UpdateGrenadeCount();
        UpdateSpecialStatus();
        UpdateShieldStatus(); // Nova fun��o para o escudo
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null && lifeScript != null)
        {
            // O valor da barra de vida � igual � vida atual do jogador
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
            // Ativa ou desativa a imagem do especial com base na vari�vel booleana 'especial'
            specialActiveImage.SetActive(playerController.especial);
        }
    }

    // ---
    // NOVO M�TODO PARA O ESCUDO
    // ---
    private void UpdateShieldStatus()
    {
        if (shieldImage != null && lifeScript != null)
        {
            // Ativa ou desativa a imagem do escudo com base na vari�vel 'ignorarDano'
            shieldImage.SetActive(lifeScript.ignorarDano);
        }
    }
}