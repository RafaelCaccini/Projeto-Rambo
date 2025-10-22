using UnityEngine;
using System.Collections;

public class TanqueSentinela : MonoBehaviour
{
    [Header("Configuração Geral")]
    public string nomeDoJogador = "Rambo"; // nome do jogador que o tanque vai procurar

    [Header("Configuração de Disparo")]
    public GameObject projetilPrefab; // o projétil que o tanque dispara
    public Transform pontoDeDisparo; // lugar de onde sai o tiro
    public float forcaTiro = 500f; // força do tiro
    public float intervaloTiros = 2f; // tempo entre os tiros

    [Header("Detecção Do Jogador")]
    public float raioDeteccao = 10f; // alcance que o tanque enxerga o jogador

    [Header("Animação de Recuo do Cano")]
    public Transform canoTanque; // parte do cano que recua ao atirar
    public float fatorRecuo = 0.8f; // quanto o cano recua
    public float duracaoRecuo = 0.1f; // tempo do recuo
    public float duracaoVolta = 0.1f; // tempo pra voltar ao normal

    private Transform jogador; // referência do jogador
    private float ultimoDisparo = 0f; // guarda o tempo do último tiro
    private Vector3 escalaOriginal; // tamanho original do cano
    private bool animando = false; // se a animação de recuo está rolando

    void Start()
    {
        if (canoTanque != null)
            escalaOriginal = canoTanque.localScale; // salva o tamanho original do cano

        GameObject objJogador = GameObject.Find(nomeDoJogador); // procura o jogador pelo nome
        if (objJogador != null)
            jogador = objJogador.transform; // guarda a posição dele
        else
            Debug.LogWarning($"Jogador '{nomeDoJogador}' não encontrado na cena!"); // avisa se não achou
    }

    void Update()
    {
        if (jogador == null) return; // se não achou jogador, não faz nada

        // vê pra que lado o tanque está virado
        int direcaoTanque = (int)Mathf.Sign(pontoDeDisparo.position.x - canoTanque.position.x);

        // vê pra que lado está o jogador
        int direcaoJogador = (int)Mathf.Sign(jogador.position.x - canoTanque.position.x);

        float distancia = Vector2.Distance(transform.position, jogador.position); // mede a distância

        // se o jogador estiver perto e na frente do tanque...
        if (distancia <= raioDeteccao && direcaoTanque == direcaoJogador)
        {
            // e se passou o tempo do último tiro
            if (Time.time - ultimoDisparo >= intervaloTiros)
            {
                Atirar(); // atira
                ultimoDisparo = Time.time; // atualiza o tempo
            }
        }
    }

    void Atirar()
    {
        // se faltar alguma coisa, não atira
        if (projetilPrefab == null || pontoDeDisparo == null || jogador == null) return;

        // cria o projétil
        GameObject proj = Instantiate(projetilPrefab, pontoDeDisparo.position, Quaternion.identity);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = 1f; // ativa a gravidade no tiro

            Vector2 origem = pontoDeDisparo.position; // de onde saiu
            Vector2 alvo = jogador.position; // pra onde vai
            Vector2 d = alvo - origem; // diferença de posição

            float g = Mathf.Abs(Physics2D.gravity.y) * rb.gravityScale; // gravidade
            float speed = forcaTiro;

            float dx = d.x;
            float dy = d.y;

            float s2 = speed * speed;
            float underRoot = s2 * s2 - g * (g * dx * dx + 2 * dy * s2); // cálculo físico

            Vector2 vel;

            if (underRoot >= 0f && Mathf.Abs(dx) > 0.01f)
            {
                float root = Mathf.Sqrt(underRoot);

                // calcula o ângulo do tiro
                float tanTheta = (s2 + root) / (g * Mathf.Abs(dx));
                float angle = Mathf.Atan(tanTheta);

                // calcula a velocidade final
                float vx = Mathf.Cos(angle) * speed * Mathf.Sign(dx);
                float vy = Mathf.Sin(angle) * speed;

                vel = new Vector2(vx, vy);
            }
            else
            {
                // se não der certo o cálculo, só atira direto no jogador
                vel = d.normalized * speed;
            }

            rb.linearVelocity = vel; // aplica a velocidade no projétil

            // gira o cano na direção do tiro
            if (canoTanque != null)
            {
                float angleInRadians = Mathf.Atan2(vel.y, vel.x);
                float angleInDegrees = angleInRadians * Mathf.Rad2Deg;
                canoTanque.rotation = Quaternion.Euler(0, 0, angleInDegrees + 90f);
            }
        }

        proj.tag = "Danger2"; // marca o projétil com essa tag

        AtivarRecuo(); // ativa a animação de recuo do cano
    }

    public void AtivarRecuo()
    {
        // se não estiver animando e tiver cano, começa a animação
        if (!animando && canoTanque != null)
            StartCoroutine(AnimarRecuo());
    }

    private IEnumerator AnimarRecuo()
    {
        animando = true;

        // calcula o tamanho do cano quando recua
        Vector3 escalaRecuo = new Vector3(
            escalaOriginal.x * fatorRecuo,
            escalaOriginal.y,
            escalaOriginal.z
        );

        // anima o recuo
        float tempo = 0f;
        while (tempo < duracaoRecuo)
        {
            tempo += Time.deltaTime;
            canoTanque.localScale = Vector3.Lerp(escalaOriginal, escalaRecuo, tempo / duracaoRecuo);
            yield return null;
        }

        // anima a volta ao normal
        tempo = 0f;
        while (tempo < duracaoVolta)
        {
            tempo += Time.deltaTime;
            canoTanque.localScale = Vector3.Lerp(escalaRecuo, escalaOriginal, tempo / duracaoVolta);
            yield return null;
        }

        canoTanque.localScale = escalaOriginal;
        animando = false; // acabou a animação
    }

    void OnDrawGizmosSelected()
    {
        // mostra o alcance no editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raioDeteccao);
    }
}

// Rafael de Souza Lins
// 2025
// Tanque concluido  
