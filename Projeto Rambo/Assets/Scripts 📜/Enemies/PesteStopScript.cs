using UnityEngine;

public class pesteStopScript : MonoBehaviour
{
    // A refer�ncia para a peste que deve parar o movimento
    [Tooltip("Arraste a Peste aqui para que ela possa ser parada.")]
    public pesteScript PesteParaParar;



    public class FreezePosition : MonoBehaviour
    {
        private Vector3 initialPosition;

        void Start()
        {
            // Salva a posi��o inicial do objeto
            initialPosition = transform.position;
        }

        void Update()
        {
            // For�a a posi��o do objeto a ser sempre a mesma do in�cio do jogo
            transform.position = initialPosition;
        }
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que entrou no trigger � o jogador
        if (other.CompareTag("Player"))
        {
            // Se a refer�ncia para a Peste existe, chame o m�todo para par�-la
            if (PesteParaParar != null)
            {
                PesteParaParar.PararMovimento();
            }
        }
    }
}