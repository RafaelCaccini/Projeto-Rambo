using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class ColetavelScript : MonoBehaviour
{
    public int valorCura = 20; //Quantia de vida curada
    public int granadas = 3; //Quantia de granadas por coletável
    public float duracaoEscudo = 5f; //tempo de duração do escudo
    public bool especial = false; //se pode ou não ativar habilidade especial

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
        }

        else if (tipo == TipoColetavel.Especial)
        {
            // Ativa habilidade especial
        }
        }
    }
