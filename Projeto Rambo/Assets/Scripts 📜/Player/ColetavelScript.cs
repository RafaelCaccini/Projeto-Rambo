using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class ColetavelScript : MonoBehaviour
{
    private Vector3 posicaoInicial; // guarda onde o item come�ou

    [Header("Configura��o de Flutua��o")]
    public float velocidadeFlutuacao = 1f; // rapidez do sobe/desce
    public float alturaFlutuacao = 0.5f;   // altura que sobe e desce

    public int valorCura = 20; // quanto cura se for kit
    public int granadas = 3;   // quantas granadas d�
    public float duracaoEscudo = 5f; // tempo do escudo

    public enum TipoColetavel
    {
        Kit,
        Granada,
        Escudo,
        Especial
    }

    [Header("Configura��o do Colet�vel")]
    public TipoColetavel tipo; // define que tipo de item �
    public int valor = 1;      // quantidade gen�rica

    void Start()
    {
        posicaoInicial = transform.position; // salva a posi��o inicial
    }

    void Update()
    {
        // faz o item flutuar para cima e para baixo
        float novaPosicaoY = posicaoInicial.y + Mathf.Sin(Time.time * velocidadeFlutuacao) * alturaFlutuacao;
        transform.position = new Vector3(posicaoInicial.x, novaPosicaoY, posicaoInicial.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // se for o player
        {
            AplicarEfeito(other.gameObject); // aplica o efeito do item
            Destroy(gameObject); // remove o colet�vel
        }
    }

    void AplicarEfeito(GameObject Rambo)
    {
        if (tipo == TipoColetavel.Kit)
        {
            LifeScript vidaMaxima = Rambo.GetComponent<LifeScript>();
            if (vidaMaxima != null)
            {
                vidaMaxima.vidaAtual += valorCura; // d� vida
            }
        }
        else if (tipo == TipoColetavel.Granada)
        {
            PlayerController granada = Rambo.GetComponent<PlayerController>();
            if (granada != null)
            {
                // aumenta granadas, m�ximo 3
                granada.granadasRestantes = Mathf.Min(granada.granadasRestantes + granadas, 3);
            }
        }
        else if (tipo == TipoColetavel.Escudo)
        {
            PlayerController player = Rambo.GetComponent<PlayerController>();
            if (player != null)
            {
                player.AtivarEscudo(duracaoEscudo); // ativa escudo
            }
        }
        else if (tipo == TipoColetavel.Especial)
        {
            PlayerController especial = Rambo.GetComponent<PlayerController>();
            if (especial != null)
            {
                especial.especial = true; // ativa modo especial
            }
        }
    }
}
