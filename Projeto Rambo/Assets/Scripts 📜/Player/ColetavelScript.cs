using UnityEngine;

public class ColetavelScript : MonoBehaviour
{
    private Vector3 posicaoInicial;

    [Header("Flutuação")]
    public float velocidadeFlutuacao = 1f;
    public float alturaFlutuacao = 0.5f;

    public enum TipoColetavel { Kit, Granada, Escudo, Especial }

    [Header("Tipo de Coletável")]
    public TipoColetavel tipo;


    [Header("Valores")]
    public int valorCura = 20;
    public int granadas = 3;
    public float duracaoEscudo = 5f;

    [Header("Som do Coletável")]
    public AudioSource audioSource; // arraste aqui no prefab

    private void Start()
    {
        posicaoInicial = transform.position;
    }

    private void Update()
    {
        // Faz o item flutuar
        float novaY = posicaoInicial.y + Mathf.Sin(Time.time * velocidadeFlutuacao) * alturaFlutuacao;
        transform.position = new Vector3(posicaoInicial.x, novaY, posicaoInicial.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        AplicarEfeito(other.gameObject);

        // Toca o som antes de destruir o objeto
        if (audioSource != null && audioSource.clip != null)
        {
            // Cria um clone do AudioSource para tocar o som fora do coletável
            AudioSource clone = Instantiate(audioSource, transform.position, Quaternion.identity);
            clone.Play();
            Destroy(clone.gameObject, clone.clip.length);
        }

        Destroy(gameObject); // destrói o coletável
    }

    private void AplicarEfeito(GameObject player)
    {
        switch (tipo)
        {
            case TipoColetavel.Kit:
                LifeScript vida = player.GetComponent<LifeScript>();
                if (vida != null)
                    vida.vidaAtual += valorCura;
                break;

            case TipoColetavel.Granada:
                PlayerController pc = player.GetComponent<PlayerController>();
                if (pc != null)
                    pc.granadasRestantes = Mathf.Min(pc.granadasRestantes + granadas, 3);
                break;

            case TipoColetavel.Escudo:
                PlayerController escudoPlayer = player.GetComponent<PlayerController>();
                if (escudoPlayer != null)
                    escudoPlayer.AtivarEscudo(duracaoEscudo);
                break;

            case TipoColetavel.Especial:
                PlayerController especialPlayer = player.GetComponent<PlayerController>();
                if (especialPlayer != null)
                    especialPlayer.especial = true;
                break;
        }
    }
}
