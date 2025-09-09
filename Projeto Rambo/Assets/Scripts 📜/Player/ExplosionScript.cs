using UnityEngine;
using static System.Net.WebRequestMethods;

public class ExplosionScript : MonoBehaviour
{
    [Header("Explos�o")]
    public GameObject objetoExplosao; // prefab do objeto que vai spawnar
    public float tempoAteExplodir = 2f;
    public float duracaoExplosao = 0.3f; // tempo que o objeto de explos�o fica na cena

    private void Start()
    {
        // Inicia a contagem pra explos�o
        Invoke(nameof(Explodir), tempoAteExplodir);
    }

    void Explodir()
    {
        // Instancia o efeito/objeto da explos�o
        if (objetoExplosao != null)
        {
            GameObject efeito = Instantiate(objetoExplosao, transform.position, Quaternion.identity);

            // Destroi o efeito depois de "duracaoExplosao" segundos
            Destroy(efeito, duracaoExplosao);
        }

        // Destroi a granada
        Destroy(gameObject);
    }
}
