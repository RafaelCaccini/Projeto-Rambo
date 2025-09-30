using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 10f; // tempo que a bala vai existir
    public GameObject danoPrefab; // O prefab que causa dano

    void Start()
    {
        Destroy(gameObject, lifeTime); // destrói a bala depois do tempo definido
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // verifica se colidiu com uma caixa
        CaixaTeste caixa = col.collider.GetComponent<CaixaTeste>();
        if (caixa != null)
        {
            caixa.DestruirCaixa(); // faz a caixa gerar item e sumir
        }

        // NOVO: Verifica se a colisão é com o Espartano
        Espartano espartano = col.collider.GetComponent<Espartano>();
        if (espartano != null)
        {
            // Se a gente bateu em um Espartano...
            bool espartanoViradoParaDireita = espartano.EstaViradoParaDireita;
            float direcaoDoTiro = transform.position.x - espartano.transform.position.x;

            bool hitNasCostas = false;

            // Vê se acertou nas costas
            if (espartanoViradoParaDireita && direcaoDoTiro < 0)
            {
                hitNasCostas = true;
            }
            else if (!espartanoViradoParaDireita && direcaoDoTiro > 0)
            {
                hitNasCostas = true;
            }

            if (hitNasCostas)
            {
                // Spawna o objeto de dano pra ser detectado pelo script de vida do Espartano
                if (danoPrefab != null)
                {
                    GameObject danoObjeto = Instantiate(danoPrefab, espartano.transform.position, Quaternion.identity);
                    danoObjeto.tag = "Danger";
                    Destroy(danoObjeto, 0.1f);
                }
            }
            else
            {
                Debug.Log("O escudo bloqueou! O Espartano não tomou dano.");
            }
        }

        Destroy(gameObject); // destrói a bala sempre que colide
    }
}