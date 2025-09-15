using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using System.Collections; // Adicionado para a coroutine

public class ColetavelScript : MonoBehaviour
{
    // Adicionado para a flutuação
    private Vector3 posicaoInicial;
    [Header("Configuração de Flutuação")]
    public float velocidadeFlutuacao = 1f;
    public float alturaFlutuacao = 0.5f;

    public int valorCura = 20; //Quantia de vida curada
    public int granadas = 3; //Quantia de granadas por coletável
    public float duracaoEscudo = 5f; //tempo de duração do escudo

    public enum TipoColetavel //Separa os tipos de coletaveis por "Categorias"
    {
        Kit,
        Granada,
        Escudo,
        Especial
    }
    //Escolhe o tipo de coletável
    [Header("Configuração do Coletável")]
    public TipoColetavel tipo;
    public int valor = 1;

    //[Header("Feedback Visual/Sonoro")]
    //public GameObject efeitoColeta; 
    //public AudioClip somColeta;

    void Start()
    {
        // Salva a posição inicial quando o objeto é criado
        posicaoInicial = transform.position;
    }

    void Update()
    {
        // Calcula a nova posição Y usando a função seno para um movimento suave de flutuação.
        float novaPosicaoY = posicaoInicial.y + Mathf.Sin(Time.time * velocidadeFlutuacao) * alturaFlutuacao;

        // Atualiza a posição do objeto, mantendo X e Z constantes.
        transform.position = new Vector3(posicaoInicial.x, novaPosicaoY, posicaoInicial.z);
    }

    private void OnTriggerEnter2D(Collider2D other) //Compara a colisão e ve c tem a tag Player
    {
        if (other.CompareTag("Player"))
        {
            AplicarEfeito(other.gameObject);

            //if(efeitoColeta)
            //if(som coleta)

            Destroy(gameObject);
        }
    }

    void AplicarEfeito(GameObject Rambo) //Aplica os efeitos conforme a seleção de tipos
    {
        if (tipo == TipoColetavel.Kit)
        {
            // Aumenta vida do jogador
            LifeScript vidaMaxima = Rambo.GetComponent<LifeScript>();
            if (vidaMaxima != null)
            {
                vidaMaxima.vidaAtual += valorCura;// Cura o jogador
            }
        }
        //Granadas
        else if (tipo == TipoColetavel.Granada)
        {
            // Aumenta munição de granada
            PlayerController granada = Rambo.GetComponent<PlayerController>();
            if (granada != null)
            {
                // Adiciona as granadas, mas limita o máximo a 3
                granada.granadasRestantes = Mathf.Min(granada.granadasRestantes + granadas, 3);
            }
        }

        else if (tipo == TipoColetavel.Escudo)
        {
            // Aplica escudo do jogador
            LifeScript vidaMaxima = Rambo.GetComponent<LifeScript>();
            if (vidaMaxima != null)
            {
                vidaMaxima.ignorarDano = true; // Ativa o escudo
                // Desativa o escudo após a duração usando uma Coroutine para mais controle.
                StartCoroutine(DesativarEscudo(vidaMaxima, duracaoEscudo));
            }
        }

        else if (tipo == TipoColetavel.Especial)
        {
            // Ativa habilidade especial
            PlayerController especial = Rambo.GetComponent<PlayerController>();
            if (especial != null)
            {
                especial.especial = true; // Ativa a habilidade especial
            }
        }
    }

    // Coroutine para desativar o escudo após um tempo determinado
    private IEnumerator DesativarEscudo(LifeScript vidaScript, float duracao)
    {
        yield return new WaitForSeconds(duracao);
        if (vidaScript != null)
        {
            vidaScript.ignorarDano = false;
        }
    }
}