using UnityEngine;

public class Projeteil : MonoBehaviour
{
    public float tempoDeVida = 6f;
    public int dano = 20;

    void Start()
    {
        // Garante que o projétil será destruído após o tempoDeVida
        Destroy(gameObject, tempoDeVida);

        // *** IMPORTANTE: A lógica de Physics2D.IgnoreCollision deve ser chamada
        // no script ChefeHiller após o Instantiate.***
        // Remova a lógica de Physics2D.IgnoreCollision daqui para focar na
        // colisão principal, e evitar o uso lento do GameObject.Find em cada projétil.
    }

    void OnCollisionEnter2D(Collision2D colisao)
    {
        GameObject other = colisao.gameObject;

        if (other.CompareTag("Player"))
        {
            // Código de dano ao Player (mantenha a versão que funciona para você)
            Debug.Log("Player atingido pelo projétil — dano: " + dano);

            Destroy(gameObject);
            return;
        }

        // Se o projétil bater no chão (que deveria ter sido ignorado, mas falhou), 
        // ou em qualquer outra parede/objeto que NÃO É O JOGADOR, ele deve ser destruído.
        // A lógica de Physics2D.IgnoreCollision é para evitar a colisão, não a destruição.

        // Se o tiro não varar, significa que a Colisão foi detectada. Destrua-o.

        // Para garantir que o tiro não seja destruído se colidir com o objeto que o criou (o próprio Hiller),
        // o que pode acontecer se o ponto de spawn estiver muito perto:
        if (!other.CompareTag("Boss") && !other.CompareTag("Untagged"))
        {
            Destroy(gameObject);
        }

        // Se você tiver certeza que o chaoDoHillerRoot tem a tag "Ground" ou "Platform"
        // você pode usar essa verificação, mas o mais seguro é Destruir:
        // Destroy(gameObject); 
    }
}