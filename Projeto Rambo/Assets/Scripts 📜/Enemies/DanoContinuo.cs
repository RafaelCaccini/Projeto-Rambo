using UnityEngine;

public class DanoContinuo : MonoBehaviour
{
    // quanto de dano vai dar a cada "tick" (repetição do efeito)
    public int danoPorTick = 3;

    // por quanto tempo o dano vai durar no total
    public float duracaoTotalDoDano = 3.0f;

    // tempo entre cada aplicação de dano
    public float intervaloDeDano = 1.0f;

    // velocidade do projétil e tempo que ele dura antes de sumir
    public float velocidadeProjetil = 10f;
    public float tempoDeVida = 5f;

    void Start()
    {
        // se o projétil não acertar nada, ele se destrói sozinho depois de um tempo
        Destroy(gameObject, tempoDeVida);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // se bater no player
        if (collision.gameObject.CompareTag("Player"))
        {
            // pega o script de vida do jogador
            LifeScript life = collision.gameObject.GetComponent<LifeScript>();

            if (life != null)
            {
                // inicia o efeito de dano contínuo no jogador
                life.IniciarDanoPorTempo(danoPorTick, duracaoTotalDoDano, intervaloDeDano);

                Debug.Log($"Projetil acertou e iniciou dano por tempo no Player por {duracaoTotalDoDano} segundos.");
            }

            // destrói o projétil depois de causar o efeito
            Destroy(gameObject);
        }
        // se bater no chão ou parede, apenas se destrói
        else if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
