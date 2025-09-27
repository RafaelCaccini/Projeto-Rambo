using UnityEngine;

public class Projeteil : MonoBehaviour
{
    public float tempoDeVida = 6f; // tempo até o projétil sumir sozinho
    public int dano = 20;          // quanto de dano ele causa

    void Start()
    {
        // faz o projétil sumir depois de um tempo
        Destroy(gameObject, tempoDeVida);

        // ⚠️ ignorar colisões com o chefe e chão é feito no script do chefe
    }

    void OnCollisionEnter2D(Collision2D colisao)
    {
        GameObject other = colisao.gameObject; // pega com quem bateu

        // se acertar o jogador
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player levou " + dano + " de dano");
            Destroy(gameObject); // destrói o projétil depois do acerto
            return;
        }

        // se bater em qualquer outra coisa que não seja o chefe ou algo sem tag
        if (!other.CompareTag("Boss") && !other.CompareTag("Untagged"))
        {
            Destroy(gameObject); // destrói o projétil
        }
    }
}
