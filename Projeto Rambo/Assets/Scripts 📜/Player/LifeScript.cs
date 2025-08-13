using UnityEngine;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    [Header("Configurações de Vida")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Feedback de Dano")]
    [SerializeField] private Renderer objectRenderer; // Renderer do objeto
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

    private Color originalColor;
    private bool isFlashing = false;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (objectRenderer == null)
        {
            objectRenderer = GetComponent<Renderer>();
        }

        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
    }

    /// <summary>
    /// Aplica dano ao objeto.
    /// </summary>
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (objectRenderer != null)
        {
            StartCoroutine(FlashDamage());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Cura o objeto.
    /// </summary>
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    /// <summary>
    /// Retorna a porcentagem de vida (0 a 1).
    /// </summary>
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    private IEnumerator FlashDamage()
    {
        if (isFlashing) yield break;
        isFlashing = true;

        objectRenderer.material.color = damageColor;
        yield return new WaitForSeconds(flashDuration);
        objectRenderer.material.color = originalColor;

        isFlashing = false;
    }

    /// <summary>
    /// Lida com a morte do objeto.
    /// </summary>
    private void Die()
    {
        // Aqui você pode colocar animação, desativar o objeto, etc.
        Debug.Log($"{gameObject.name} morreu.");
        Destroy(gameObject);
    }
}
