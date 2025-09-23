using UnityEngine;

public class Templario : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float velocidade = 3f;

    [Header("Configurações de Ataque")]
    public GameObject objetoDeDanoPrefab; // O objeto que será instanciado para aplicar dano
    private bool estaAtacando = false;

    private Transform jogadorTransform;

    void Start()
    {
        // Tenta encontrar o objeto do jogador na cena pela tag "Player"
        GameObject jogadorObjeto = GameObject.FindGameObjectWithTag("Player");

        if (jogadorObjeto != null)
        {
            // Pega a referência à Transform do jogador
            jogadorTransform = jogadorObjeto.transform;
        }
        else
        {
            Debug.LogWarning("Jogador com a tag 'Player' não encontrado. O Templário não se moverá.");
        }
    }

    void Update()
    {
        // Se o Templário não estiver atacando e o jogador foi encontrado,
        // ele continua se movendo para a esquerda
        if (!estaAtacando && jogadorTransform != null)
        {
            transform.Translate(Vector2.left * velocidade * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se a colisão é com o jogador
        if (collision.gameObject.CompareTag("Player"))
        {
            // Para o movimento do Templário
            estaAtacando = true;

            // Spawna o objeto de dano no mesmo local da colisão
            if (objetoDeDanoPrefab != null)
            {
                GameObject objetoDeDano = Instantiate(objetoDeDanoPrefab, transform.position, Quaternion.identity);
                // Atribui a tag "Danger" ao objeto recém-instanciado
                objetoDeDano.tag = "Danger";

                // Destrói o objeto de dano após um pequeno tempo,
                // para que ele não fique na cena.
                Destroy(objetoDeDano, 0.1f);
            }

            // O Templário não será mais destruído aqui. 
            // Ele permanecerá na cena até ser destruído por outra lógica (ex: tomar dano).
        }
    }
}