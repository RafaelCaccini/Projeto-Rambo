using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class ColetavelScript : MonoBehaviour
{
    private Vector3 posicaoInicial;
    [Header("Configuração de Flutuação")]
    public float velocidadeFlutuacao = 1f;
    public float alturaFlutuacao = 0.5f;

    public int valorCura = 20;
    public int granadas = 3;
    public float duracaoEscudo = 5f;

    public enum TipoColetavel
    {
        Kit,
        Granada,
        Escudo,
        Especial
    }

    [Header("Configuração do Coletável")]
    public TipoColetavel tipo;
    public int valor = 1;

    void Start()
    {
        posicaoInicial = transform.position;
    }

    void Update()
    {
        float novaPosicaoY = posicaoInicial.y + Mathf.Sin(Time.time * velocidadeFlutuacao) * alturaFlutuacao;
        transform.position = new Vector3(posicaoInicial.x, novaPosicaoY, posicaoInicial.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AplicarEfeito(other.gameObject);
            Destroy(gameObject);
        }
    }

    void AplicarEfeito(GameObject Rambo)
    {
        if (tipo == TipoColetavel.Kit)
        {
            LifeScript vidaMaxima = Rambo.GetComponent<LifeScript>();
            if (vidaMaxima != null)
            {
                vidaMaxima.vidaAtual += valorCura;
            }
        }
        else if (tipo == TipoColetavel.Granada)
        {
            PlayerController granada = Rambo.GetComponent<PlayerController>();
            if (granada != null)
            {
                granada.granadasRestantes = Mathf.Min(granada.granadasRestantes + granadas, 3);
            }
        }
        else if (tipo == TipoColetavel.Escudo)
        {
            PlayerController player = Rambo.GetComponent<PlayerController>();
            if (player != null)
            {
                // Chama a nova função no PlayerController para gerenciar o escudo
                player.AtivarEscudo(duracaoEscudo);
            }
        }
        else if (tipo == TipoColetavel.Especial)
        {
            PlayerController especial = Rambo.GetComponent<PlayerController>();
            if (especial != null)
            {
                especial.especial = true;
            }
        }
    }
}