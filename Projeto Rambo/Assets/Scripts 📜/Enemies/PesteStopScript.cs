using UnityEngine;

public class pesteStopScript : MonoBehaviour
{
    // Referência para o script da peste que será parada ao colidir com o player.
    // Você precisa arrastar no Inspector o objeto que tem o "pesteScript" para que funcione.
    [Tooltip("Arraste a Peste aqui para que ela possa ser parada.")]
    public pesteScript PesteParaParar;



    // Classe interna que congela a posição de um objeto.
    // Obs: como está dentro de pesteStopScript, ela só vai funcionar se for usada dentro desse script.
    public class FreezePosition : MonoBehaviour
    {
        private Vector3 initialPosition; // Armazena a posição inicial do objeto

        void Start()
        {
            // Guarda a posição inicial do objeto assim que o jogo começa
            initialPosition = transform.position;
        }

        void Update()
        {
            // Mantém o objeto travado sempre na mesma posição (sem se mover)
            transform.position = initialPosition;
        }
    }



    // Detecta quando algo entra na área de trigger (precisa de um Collider2D com "Is Trigger" marcado)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se quem entrou no trigger tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            // Se a referência da peste estiver atribuída no Inspector...
            if (PesteParaParar != null)
            {
                // ... chama o método que deve parar o movimento dela
                PesteParaParar.PararMovimento();
            }
        }
    }
}
