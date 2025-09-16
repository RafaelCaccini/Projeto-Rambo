using UnityEngine;

public class Projeteil : MonoBehaviour
{
    public float tempoDeVida = 6f;
    public int dano = 20;

    void Start()
    {
        // Garante que o proj�til ser� destru�do ap�s o tempoDeVida
        Destroy(gameObject, tempoDeVida);

        // *** IMPORTANTE: A l�gica de Physics2D.IgnoreCollision deve ser chamada
        // no script ChefeHiller ap�s o Instantiate.***
        // Remova a l�gica de Physics2D.IgnoreCollision daqui para focar na
        // colis�o principal, e evitar o uso lento do GameObject.Find em cada proj�til.
    }

    void OnCollisionEnter2D(Collision2D colisao)
    {
        GameObject other = colisao.gameObject;

        if (other.CompareTag("Player"))
        {
            // C�digo de dano ao Player (mantenha a vers�o que funciona para voc�)
            Debug.Log("Player atingido pelo proj�til � dano: " + dano);

            Destroy(gameObject);
            return;
        }

        // Se o proj�til bater no ch�o (que deveria ter sido ignorado, mas falhou), 
        // ou em qualquer outra parede/objeto que N�O � O JOGADOR, ele deve ser destru�do.
        // A l�gica de Physics2D.IgnoreCollision � para evitar a colis�o, n�o a destrui��o.

        // Se o tiro n�o varar, significa que a Colis�o foi detectada. Destrua-o.

        // Para garantir que o tiro n�o seja destru�do se colidir com o objeto que o criou (o pr�prio Hiller),
        // o que pode acontecer se o ponto de spawn estiver muito perto:
        if (!other.CompareTag("Boss") && !other.CompareTag("Untagged"))
        {
            Destroy(gameObject);
        }

        // Se voc� tiver certeza que o chaoDoHillerRoot tem a tag "Ground" ou "Platform"
        // voc� pode usar essa verifica��o, mas o mais seguro � Destruir:
        // Destroy(gameObject); 
    }
}