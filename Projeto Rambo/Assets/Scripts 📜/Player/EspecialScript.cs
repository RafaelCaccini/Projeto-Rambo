using UnityEngine;

public class EspecialScript : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private int feicheHash = Animator.StringToHash("Feiche");

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        SetVisivel(false);
    }

    private void SetVisivel(bool visivel)
    {
        if (spriteRenderer == null) return;
        Color c = spriteRenderer.color;
        c.a = visivel ? 1f : 0f;
        spriteRenderer.color = c;
    }

    public void AtivarEspecial()
    {
        SetVisivel(true);
        animator.SetTrigger(feicheHash);
    }

    // Chamado via Animation Event no final da animação
    public void EspecialAcabou()
    {
        SetVisivel(false);
    }
}
