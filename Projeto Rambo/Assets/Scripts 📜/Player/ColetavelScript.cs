using UnityEngine;

public class ColetavelScript : MonoBehaviour
{
    private bool coletado = false;

    [Header("Animação ao Coletar")]
    public float velocidadeSubida = 1.5f;
    public float tempoDesaparecer = 0.4f;

    public enum TipoColetavel { Kit, Granada, Escudo, Especial }

    [Header("Tipo de Coletável")]
    public TipoColetavel tipo;

    [Header("Valores")]
    public int valorCura = 20;
    public int granadas = 3;
    public float duracaoEscudo = 5f;

    [Header("Som do Coletável")]
    public AudioSource audioSource; // arraste seu AudioSource aqui no prefab

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (coletado) return;                          // evita Coleta dupla
        if (!other.CompareTag("Player")) return;

        coletado = true;                               // trava na hora

        // desativa o collider para impedir nova colisão
        Collider2D col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        AplicarEfeito(other.gameObject);
        TocarSom();

        // inicia a animação de sumir
        StartCoroutine(SubirEDesaparecer());
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

    private void TocarSom()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            AudioSource clone = Instantiate(audioSource, transform.position, Quaternion.identity);
            clone.Play();
            Destroy(clone.gameObject, clone.clip.length);
        }
    }

    private System.Collections.IEnumerator SubirEDesaparecer()
    {
        float timer = 0f;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color cor = sr.color;

        while (timer < tempoDesaparecer)
        {
            // sobe suavemente
            transform.position += Vector3.up * velocidadeSubida * Time.deltaTime;

            // fade-out
            cor.a = Mathf.Lerp(1f, 0f, timer / tempoDesaparecer);
            sr.color = cor;

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
