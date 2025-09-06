using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class ColetavelScript : MonoBehaviour
{
    public int valorCura = 20; //Quantia de vida curada
    public int granadas = 3; //Quantia de granadas por colet�vel
    public float duracaoEscudo = 5f; //tempo de dura��o do escudo
    public bool especial = false; //se pode ou n�o ativar habilidade especial

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
    //public GameObject efeitoColeta; 
    //public AudioClip somColeta;

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
        }

        else if (tipo == TipoColetavel.Especial)
        {
            // Ativa habilidade especial
        }
        }
    }
