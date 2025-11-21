using UnityEngine;

public class DanoContinuo : MonoBehaviour
{
    public int danoPorTick = 3;
    public float duracaoTotalDoDano = 3.0f;
    public float intervaloDeDano = 1.0f;

    public float velocidadeProjetil = 10f;
    public float tempoDeVida = 5f;

    public Animator anim;

    private bool jaQuebrou = false;

    void Start()
    {
        Destroy(gameObject, tempoDeVida);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (jaQuebrou) return; // evita ativar mais de uma vez

        jaQuebrou = true;

        // Ativa animação
        if (anim != null)
            anim.SetTrigger("Quebra");

        // Se acertar o player, aplicar o efeito antes de quebrar
        if (collision.gameObject.CompareTag("Player"))
        {
            LifeScript life = collision.gameObject.GetComponent<LifeScript>();

            if (life != null)
                life.IniciarDanoPorTempo(danoPorTick, duracaoTotalDoDano, intervaloDeDano);
        }
    }

    // 🔥 Chamado pela animação através de Animation Event
    public void DestruirObjeto()
    {
        Destroy(gameObject);
    }
}
