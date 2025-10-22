using UnityEngine;
using UnityEngine.SceneManagement;

public class CorpoAnimationEvents : MonoBehaviour
{
    private PlayerController player;

    void Awake()
    {
        player = GetComponentInParent<PlayerController>();
    }

    // Chamado por Animation Event durante a animação de morte
    public void IniciarMortePlayer()
    {
        if (player != null && player.animatorPerna != null)
        {
            var renderers = player.animatorPerna.GetComponentsInChildren<SpriteRenderer>();
            foreach (var r in renderers)
            {
                Color c = r.color;
                c.a = 0f; // garante invisibilidade total
                r.color = c;
            }
        }
    }

    // Chamado no fim da animação de morte (Animation Event)
    public void ChamarCenaDeMorte()
    {
        SceneManager.LoadScene("Morte"); // troca pra cena de morte
    }
}
