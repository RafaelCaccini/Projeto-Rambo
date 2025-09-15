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
        if (projetilPrefab == null || pontoDeDisparo == null || jogador == null) return;

        GameObject proj = Instantiate(projetilPrefab, pontoDeDisparo.position, Quaternion.identity);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = 1f;

            Vector2 origem = pontoDeDisparo.position;
            Vector2 alvo = jogador.position;
            Vector2 d = alvo - origem;

            float g = Mathf.Abs(Physics2D.gravity.y) * rb.gravityScale;
            float speed = forcaTiro;

            float dx = d.x;
            float dy = d.y;

            float s2 = speed * speed;
            float underRoot = s2 * s2 - g * (g * dx * dx + 2 * dy * s2);

            Vector2 vel;

            if (underRoot >= 0f && Mathf.Abs(dx) > 0.01f)
            {
                float root = Mathf.Sqrt(underRoot);

                // MUDANÇA AQUI: Usando o sinal de mais para o ângulo alto.
                float tanTheta = (s2 + root) / (g * dx);

                float angle = Mathf.Atan(tanTheta);

                float vx = Mathf.Cos(angle) * speed * Mathf.Sign(dx);
                float vy = Mathf.Sin(angle) * speed;

                vel = new Vector2(vx, vy);
            }
            else
            {
                // Se não houver solução física (a força é muito baixa ou o alvo está muito perto), atira em direção ao player.
                vel = d.normalized * speed;
            }

            rb.linearVelocity = vel;

            if (canoTanque != null)
            {
                float angleInRadians = Mathf.Atan2(vel.y, vel.x);
                float angleInDegrees = angleInRadians * Mathf.Rad2Deg;
                canoTanque.rotation = Quaternion.Euler(0, 0, angleInDegrees + 90f);
            }
        }

        proj.tag = "Danger2";

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