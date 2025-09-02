using UnityEngine;
using System.Collections;

public class TanqueSentinela : MonoBehaviour
{
    [Header("Configuração Geral")]
    public string nomeDoJogador = "Rambo";

    [Header("Configuração de Disparo")]
    public GameObject projetilPrefab;
    public Transform pontoDeDisparo;
    public float forcaTiro = 500f;
    public float intervaloTiros = 2f;

    [Header("Detecção Do Jogador")]
    public float raioDeteccao = 10f;

    [Header("Animação de Recuo do Cano")]
    public Transform canoTanque;
    public float fatorRecuo = 0.8f;
    public float duracaoRecuo = 0.1f;
    public float duracaoVolta = 0.1f;

    private Transform jogador;
    private float ultimoDisparo = 0f;
    private Vector3 escalaOriginal;
    private bool animando = false;

    void Start()
    {
        if (canoTanque != null)
            escalaOriginal = canoTanque.localScale;

        GameObject objJogador = GameObject.Find(nomeDoJogador);
        if (objJogador != null)
            jogador = objJogador.transform;
        else
            Debug.LogWarning($"Jogador '{nomeDoJogador}' não encontrado na cena!");
    }

    void Update()
    {
        if (jogador == null) return;

        float distancia = Vector2.Distance(transform.position, jogador.position);
        if (distancia <= raioDeteccao)
        {
            if (Time.time - ultimoDisparo >= intervaloTiros)
            {
                Atirar();
                ultimoDisparo = Time.time;
            }
        }
    }

    void Atirar()
    {
        if (projetilPrefab == null || pontoDeDisparo == null) return;

        GameObject proj = Instantiate(projetilPrefab, pontoDeDisparo.position, pontoDeDisparo.rotation);

        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 1f;
            rb.AddForce(pontoDeDisparo.right * forcaTiro);
        }

        proj.tag = "Danger";

        // Faz o projétil sumir sozinho após 2 segundos
        Destroy(proj, 2f);

        // Recuo do cano
        AtivarRecuo();
    }

    public void AtivarRecuo()
    {
        if (!animando && canoTanque != null)
            StartCoroutine(AnimarRecuo());
    }

    private IEnumerator AnimarRecuo()
    {
        animando = true;

        Vector3 escalaRecuo = new Vector3(
            escalaOriginal.x * fatorRecuo,
            escalaOriginal.y,
            escalaOriginal.z
        );

        float tempo = 0f;
        while (tempo < duracaoRecuo)
        {
            tempo += Time.deltaTime;
            canoTanque.localScale = Vector3.Lerp(escalaOriginal, escalaRecuo, tempo / duracaoRecuo);
            yield return null;
        }

        tempo = 0f;
        while (tempo < duracaoVolta)
        {
            tempo += Time.deltaTime;
            canoTanque.localScale = Vector3.Lerp(escalaRecuo, escalaOriginal, tempo / duracaoVolta);
            yield return null;
        }

        canoTanque.localScale = escalaOriginal;
        animando = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raioDeteccao);
    }
}
