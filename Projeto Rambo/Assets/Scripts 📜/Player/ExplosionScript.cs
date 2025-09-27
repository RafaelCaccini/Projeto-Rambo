using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    [Header("Explosão")]
    public GameObject objetoExplosao; // prefab do efeito de explosão
    public float tempoAteExplodir = 2f; // quanto tempo esperar antes de explodir
    public float duracaoExplosao = 0.3f; // quanto tempo o efeito fica visível

    private void Start()
    {
        // Começa a contagem pra explosão
        Invoke(nameof(Explodir), tempoAteExplodir);
    }

    void Explodir()
    {
        // Cria o efeito da explosão na posição atual
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
