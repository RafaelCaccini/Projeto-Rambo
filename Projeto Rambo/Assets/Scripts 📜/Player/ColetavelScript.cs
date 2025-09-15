using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using System.Collections; // Adicionado para a coroutine

public class ColetavelScript : MonoBehaviour
{
    // Adicionado para a flutua��o
    private Vector3 posicaoInicial;
    [Header("Configura��o de Flutua��o")]
    public float velocidadeFlutuacao = 1f;
    public float alturaFlutuacao = 0.5f;

    public int valorCura = 20; //Quantia de vida curada
    public int granadas = 3; //Quantia de granadas por colet�vel
    public float duracaoEscudo = 5f; //tempo de dura��o do escudo

    public enum TipoColetavel //Separa os tipos de coletaveis por "Categorias"
    {
        Kit,
        Granada,
        Escudo,
        Especial
    }
    //Escolhe o tipo de colet�vel
    [Header("Configura��o do Colet�vel")]
    public TipoColetavel tipo;
    public int valor = 1;

    //[Header("Feedback Visual/Sonoro")]
    //public GameObject efeitoColeta;�
    //public AudioClip somColeta;

    void Start()
    {
        // Salva a posi��o inicial quando o objeto � criado
        posicaoInicial = transform.position;
    }

    void Update()
    {
        // Calcula a nova posi��o Y usando a fun��o seno para um movimento suave de flutua��o.
        float novaPosicaoY = posicaoInicial.y + Mathf.Sin(Time.time * velocidadeFlutuacao) * alturaFlutuacao;

        // Atualiza a posi��o do objeto, mantendo X e Z constantes.
        transform.position = new Vector3(posicaoInicial.x, novaPosicaoY, posicaoInicial.z);
    }

    private void OnTriggerEnter2D(Collider2D other) //Compara a colis�o e ve c tem a tag Player
    {
        if (other.CompareTag("Player"))
        {
            AplicarEfeito(other.gameObject);

            //if(efeitoColeta)
            //if(som coleta)

            Destroy(gameObject);
        }
    }

    void AplicarEfeito(GameObject Rambo) //Aplica os efeitos conforme a sele��o de tipos
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
            // Aumenta muni��o de granada
            PlayerController granada = Rambo.GetComponent<PlayerController>();
            if (granada != null)
            {
                // Adiciona as granadas, mas limita o m�ximo a 3
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
                // Desativa o escudo ap�s a dura��o usando uma Coroutine para mais controle.
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

    // Coroutine para desativar o escudo ap�s um tempo determinado
    private IEnumerator DesativarEscudo(LifeScript vidaScript, float duracao)
    {
        yield return new WaitForSeconds(duracao);
        if (vidaScript != null)
        {
            vidaScript.ignorarDano = false;
        }
    }
}