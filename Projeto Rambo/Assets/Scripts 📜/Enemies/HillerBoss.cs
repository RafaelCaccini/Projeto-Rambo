using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChefeHiller : MonoBehaviour
{
    [Header("Jogador")]
    public Transform jogador;

    [Header("Configurações do Chefe")]
    public float raioAtivacao = 5f;   // Raio para ativar o chefe
    public float velocidade = 2f;      // Velocidade para seguir o jogador
    public bool chefeVivo = true;      // Controla se o chefe está vivo

    [Header("Soldados")]
    public GameObject prefabSoldado;   // Prefab do soldado
    public Transform[] pontosSpawn;    // Pontos para spawnar soldados
    public float intervaloSpawn = 8f;  // Intervalo entre spawns
    private List<GameObject> soldadosAtuais = new List<GameObject>();

    [Header("Bazuca")]
    public GameObject prefabProjeteil; // Prefab do projétil da bazuca
    public float cooldownTiro = 2f;    // Tempo entre tiros
    public float velocidadeTiro = 10f; // Velocidade do projétil
    private float timerTiro = 0f;

    private bool ativo = false;

    void Start()
    {
        timerTiro = 0f; // Pode atirar imediatamente
    }

    void Update()
    {
        if (!chefeVivo) return;

        // Ativa o chefe se o jogador estiver dentro do raio
        if (!ativo && Vector2.Distance(transform.position, jogador.position) <= raioAtivacao)
        {
            ativo = true;
            StartCoroutine(RotinaSpawnSoldados());
        }

        if (!ativo) return;

        // Segue o jogador
        SeguirJogador();

        // Dispara a bazuca quando o cooldown acabar
        timerTiro -= Time.deltaTime;
        if (timerTiro <= 0f)
        {
            AtirarBazuca();
            timerTiro = cooldownTiro;
        }
    }

    void SeguirJogador()
    {
        if (jogador == null) return;

        Vector2 direcao = (jogador.position - transform.position).normalized;
        transform.position += (Vector3)(direcao * velocidade * Time.deltaTime);
    }

    // Rotina para spawnar soldados continuamente
    IEnumerator RotinaSpawnSoldados()
    {
        while (chefeVivo)
        {
            SpawnarSoldados();
            yield return new WaitForSeconds(intervaloSpawn);
        }
    }

    void SpawnarSoldados()
    {
        foreach (Transform ponto in pontosSpawn)
        {
            GameObject soldado = Instantiate(prefabSoldado, ponto.position, Quaternion.identity);
            soldadosAtuais.Add(soldado);
        }
    }

    void AtirarBazuca()
    {
        if (prefabProjeteil == null || jogador == null) return;

        Vector2 direcao = (jogador.position - transform.position).normalized;
        Vector2 pontoSpawn = (Vector2)transform.position + direcao * 0.5f; // spawn 0.5 unidades à frente do chefe
        GameObject projeteil = Instantiate(prefabProjeteil, pontoSpawn, Quaternion.identity);
        Rigidbody2D rb = projeteil.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direcao * velocidadeTiro; // projétil se move na direção do jogador
        }
    }

    // Será chamado pelo seu sistema de especial
    public void RecebeuEspecial()
    {
        if (!chefeVivo) return;

        chefeVivo = false;
        Destroy(gameObject);
        Debug.Log("Chefe derrotado!");
    }

    // Mostra o raio de ativação no editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raioAtivacao);
    }
}
