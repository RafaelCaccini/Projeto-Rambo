using UnityEngine;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    //Configurações de Vida 
    [Header("Configurações de Vida")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float damageOnCollision = 10f; // Dano padrão ao colidir
    private float currentHealth;

    //Feedback Visual de Dano
    [Header("Feedback Visual de Dano")]
    [SerializeField] private Renderer[] playerRenderers; // Array para todos os renderers do jogador
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

    private Color[] originalColors; // Array para armazenar as cores originais de cada renderer
    private bool isFlashing = false;

    private void Awake()
    {
        currentHealth = maxHealth;

        // Se o array de renderers não foi preenchido, tenta pegar todos os renderers nos filhos
        if (playerRenderers == null || playerRenderers.Length == 0)
        {
            playerRenderers = GetComponentsInChildren<Renderer>();
        }

        // Armazena as cores originais de todos os renderers
        originalColors = new Color[playerRenderers.Length];
        for (int i = 0; i < playerRenderers.Length; i++)
        {
            if (playerRenderers[i] != null)
            {
                originalColors[i] = playerRenderers[i].material.color;
            }
        }
    }

   
    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return; // Se já morreu, não faz nada.

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Dano aplicado: {amount}. Vida atual: {currentHealth}.");

        // Se a vida for maior que zero, chama o efeito de flash
        if (currentHealth > 0)
        {
            StartCoroutine(FlashDamage());
        }

        // Se a vida chegou a zero, o objeto morre
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashDamage()
    {
        if (isFlashing) yield break; // Evita que a corrotina seja chamada múltiplas vezes
        isFlashing = true;

        // Altera a cor de todos os renderers para a cor de dano
        for (int i = 0; i < playerRenderers.Length; i++)
        {
            if (playerRenderers[i] != null)
            {
                playerRenderers[i].material.color = damageColor;
            }
        }

        yield return new WaitForSeconds(flashDuration);

        // Restaura a cor original de todos os renderers
        for (int i = 0; i < playerRenderers.Length; i++)
        {
            if (playerRenderers[i] != null)
            {
                playerRenderers[i].material.color = originalColors[i];
            }
        }

        isFlashing = false;
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} morreu.");
        Destroy(gameObject);
    }

    /// <summary>
    /// Detecta colisões físicas.
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        // Verifica se o objeto colidido tem a tag "Danger"
        if (collision.gameObject.CompareTag("Danger"))
        {
            Debug.Log($"Colisão com objeto perigoso: {collision.gameObject.name}. Aplicando dano.");
            TakeDamage(damageOnCollision);
        }
    }
}