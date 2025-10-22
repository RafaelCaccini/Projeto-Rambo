using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Classe respons�vel por atualizar a interface do jogador
public class UIManager : MonoBehaviour
{
    // === Refer�ncias da UI ===
    [Header("UI do Jogador")]
    public Slider healthBar;                   // Barra de vida do jogador
    public TextMeshProUGUI grenadeCountText;  // Texto que mostra quantas granadas o jogador tem
    public GameObject specialActiveImage;     // Imagem que indica se o especial est� ativo

    [Header("UI do Escudo")]
    public GameObject shieldImage;             // Imagem do escudo que aparece quando ativo

    // === Refer�ncia do Jogador ===
    private PlayerController playerController; // Refer�ncia ao script PlayerController
    private LifeScript lifeScript;             // Refer�ncia ao script LifeScript (vida do jogador)

    void Start()
    {
        // Encontra o jogador na cena (precisa ter a tag "Player")
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Pega os scripts do jogador
            playerController = player.GetComponent<PlayerController>();
            lifeScript = player.GetComponent<LifeScript>();

            // Configura o valor m�ximo da barra de vida
            if (healthBar != null && lifeScript != null)
                healthBar.maxValue = lifeScript.vidaMaxima;

            // Come�a com a imagem do escudo desativada
            if (shieldImage != null)
                shieldImage.SetActive(false);
        }
        else
        {
            // Mensagem de erro se n�o encontrar o jogador
            Debug.LogError("Jogador n�o encontrado. Certifique-se de que o objeto do jogador tem a tag 'Player'.");
        }
    }

    void Update()
    {
        // Atualiza todos os elementos da UI a cada frame
        UpdateHealthBar();
        UpdateGrenadeCount();
        UpdateSpecialStatus();
        UpdateShieldStatus(); // Atualiza a imagem do escudo
    }

    // Atualiza a barra de vida com a vida atual do jogador
    private void UpdateHealthBar()
    {
        if (healthBar != null && lifeScript != null)
        {
            healthBar.value = lifeScript.vidaAtual;
        }
    }

    // Atualiza o texto da quantidade de granadas
    private void UpdateGrenadeCount()
    {
        if (grenadeCountText != null && playerController != null)
        {
            grenadeCountText.text = playerController.granadasRestantes.ToString();
        }
    }

    // Mostra ou esconde a imagem do especial baseado no status do jogador
    private void UpdateSpecialStatus()
    {
        if (specialActiveImage != null && playerController != null)
        {
            specialActiveImage.SetActive(playerController.especial);
        }
    }

    // Mostra ou esconde a imagem do escudo baseado na vari�vel 'ignorarDano' do LifeScript
    private void UpdateShieldStatus()
    {
        if (shieldImage != null && lifeScript != null)
        {
            shieldImage.SetActive(lifeScript.ignorarDano);
        }
    }
}
