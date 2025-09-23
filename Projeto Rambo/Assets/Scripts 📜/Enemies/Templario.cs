using UnityEngine;

public class Templario : MonoBehaviour
{
    [Header("Configura��es de Movimento")]
    public float velocidade = 3f;

    [Header("Configura��es de Ataque")]
    public GameObject objetoDeDanoPrefab; // O objeto que ser� instanciado para aplicar dano
    private bool estaAtacando = false;

    private Transform jogadorTransform;

    void Start()
    {
        // Tenta encontrar o objeto do jogador na cena pela tag "Player"
        GameObject jogadorObjeto = GameObject.FindGameObjectWithTag("Player");

        if (jogadorObjeto != null)
        {
            // Pega a refer�ncia � Transform do jogador
            jogadorTransform = jogadorObjeto.transform;
        }
        else
        {
            Debug.LogWarning("Jogador com a tag 'Player' n�o encontrado. O Templ�rio n�o se mover�.");
        }
    }

    void Update()
    {
        // Se o Templ�rio n�o estiver atacando e o jogador foi encontrado,
        // ele continua se movendo para a esquerda
        if (!estaAtacando && jogadorTransform != null)
        {
            transform.Translate(Vector2.left * velocidade * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se a colis�o � com o jogador
        if (collision.gameObject.CompareTag("Player"))
        {
            // Para o movimento do Templ�rio
            estaAtacando = true;

            // Spawna o objeto de dano no mesmo local da colis�o
            if (objetoDeDanoPrefab != null)
            {
                GameObject objetoDeDano = Instantiate(objetoDeDanoPrefab, transform.position, Quaternion.identity);
                // Atribui a tag "Danger" ao objeto rec�m-instanciado
                objetoDeDano.tag = "Danger";

                // Destr�i o objeto de dano ap�s um pequeno tempo,
                // para que ele n�o fique na cena.
                Destroy(objetoDeDano, 0.1f);
            }

            // O Templ�rio n�o ser� mais destru�do aqui. 
            // Ele permanecer� na cena at� ser destru�do por outra l�gica (ex: tomar dano).
        }
    }
}