using UnityEngine;

public class pesteStopScript : MonoBehaviour
{
    // A referência para a peste que deve parar o movimento
    [Tooltip("Arraste a Peste aqui para que ela possa ser parada.")]
    public pesteScript PesteParaParar;



    public class FreezePosition : MonoBehaviour
    {
        private Vector3 initialPosition;

        void Start()
        {
            // Salva a posição inicial do objeto
            initialPosition = transform.position;
        }

        void Update()
        {
            // Força a posição do objeto a ser sempre a mesma do início do jogo
            transform.position = initialPosition;
        }
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que entrou no trigger é o jogador
        if (other.CompareTag("Player"))
        {
            // Se a referência para a Peste existe, chame o método para pará-la
            if (PesteParaParar != null)
            {
                PesteParaParar.PararMovimento();
            }
        }
    }
}