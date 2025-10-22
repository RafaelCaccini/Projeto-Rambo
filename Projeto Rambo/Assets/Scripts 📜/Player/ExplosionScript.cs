using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    [Header("Explos�o")]
    public GameObject objetoExplosao; // prefab do efeito de explos�o
    public float tempoAteExplodir = 2f; // quanto tempo esperar antes de explodir
    public float duracaoExplosao = 0.3f; // quanto tempo o efeito fica vis�vel

    private void Start()
    {
        // Come�a a contagem pra explos�o
        Invoke(nameof(Explodir), tempoAteExplodir);
    }

    void Explodir()
    {
        // Cria o efeito da explos�o na posi��o atual
        if (objetoExplosao != null)
        {
            GameObject efeito = Instantiate(objetoExplosao, transform.position, Quaternion.identity);

            // Destroi o efeito depois de um tempo
            Destroy(efeito, duracaoExplosao);
        }

        // Remove a granada (ou objeto que explodiu)
        Destroy(gameObject);
    }
}
