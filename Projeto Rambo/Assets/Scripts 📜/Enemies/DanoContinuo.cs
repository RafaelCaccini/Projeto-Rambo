using UnityEngine;

public class DanoContinuo : MonoBehaviour
{
    [Header("Configura��o de Dano por Tempo")]
    [Tooltip("Dano aplicado a cada 'tick' (dano que se repete).")]
    public int danoPorTick = 3;

    [Tooltip("Dura��o total em segundos em que o dano ser� aplicado (3.0 segundos).")]
    public float duracaoTotalDoDano = 3.0f;

    [Tooltip("Intervalo de tempo em segundos entre cada aplica��o de dano (Ex: 1.0).")]
    public float intervaloDeDano = 1.0f;

    [Header("Geral")]
    public float velocidadeProjetil = 10f;
    public float tempoDeVida = 5f;

    void Start()
    {
        // Destr�i o proj�til depois de um tempo, caso ele n�o acerte nada.
        Destroy(gameObject, tempoDeVida);
    }

    // Usado para DETECTAR O IMPACTO e iniciar o efeito. (Colis�o normal)
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Verifica se acertou o Player
        if (collision.gameObject.CompareTag("Player"))
        {
            LifeScript life = collision.gameObject.GetComponent<LifeScript>();

            if (life != null)
            {
                // **CHAMADA PRINCIPAL:** Inicia o dano por tempo no script de vida do Player.
                life.IniciarDanoPorTempo(danoPorTick, duracaoTotalDoDano, intervaloDeDano);

                Debug.Log($"Projetil acertou e iniciou dano por tempo no Player por {duracaoTotalDoDano} segundos.");
            }

            // Destr�i o proj�til imediatamente ap�s iniciar o efeito.
            Destroy(gameObject);
        }
        // 2. Verifica se acertou o Ground ou outro objeto para sumir
        else if (collision.gameObject.CompareTag("Ground"))
        {
            // Destr�i ao bater no ch�o ou parede
            Destroy(gameObject);
        }
    }
}