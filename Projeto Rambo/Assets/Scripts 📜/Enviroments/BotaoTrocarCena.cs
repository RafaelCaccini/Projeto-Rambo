using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class BotaoTrocarCena : MonoBehaviour
{
    [Header("Nome da Cena para carregar")]
    public string nomeCena;

    [Header("Configuração de Animação do Botão")]
    public Image imagemDoBotao;
    public float duracaoFadeBotao = 0.5f;

    [Header("Configuração de Animação do Zoom")]
    public bool usarAnimacaoZoom = true;
    public CanvasScaler canvasScaler;  // NOVO: Referência ao Canvas Scaler
    public float duracaoZoom = 0.5f;
    public float fatorZoom = 1.2f;

    [Header("Vinheta")]
    public Image vinheta;
    public float duracaoVinheta = 0.8f;

    private float escalaOriginal; // NOVO: Armazena a escala do Canvas Scaler
    private bool jaCliquei = false;

    void Start()
    {
        if (imagemDoBotao != null)
            imagemDoBotao.color = new Color(imagemDoBotao.color.r, imagemDoBotao.color.g, imagemDoBotao.color.b, 0);

        // NOVO: Pega a referência e guarda a escala do Canvas Scaler
        if (canvasScaler == null)
        {
            canvasScaler = FindObjectOfType<Canvas>().GetComponent<CanvasScaler>();
        }
        if (canvasScaler != null)
        {
            escalaOriginal = canvasScaler.scaleFactor;
        }

        if (vinheta != null)
            vinheta.color = new Color(vinheta.color.r, vinheta.color.g, vinheta.color.b, 0);
    }

    public void CarregarCena()
    {
        if (jaCliquei) return;
        jaCliquei = true;
        StartCoroutine(AnimarBotaoEFazerTransicao());
    }

    private IEnumerator AnimarBotaoEFazerTransicao()
    {
        if (imagemDoBotao != null)
        {
            float t = 0;
            Color corBotao = imagemDoBotao.color;
            while (t < duracaoFadeBotao)
            {
                t += Time.deltaTime;
                float alpha = Mathf.Lerp(0, 1, t / duracaoFadeBotao);
                imagemDoBotao.color = new Color(corBotao.r, corBotao.g, corBotao.b, alpha);
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(TransicaoCena());
    }

    private IEnumerator TransicaoCena()
    {
        if (vinheta != null)
        {
            vinheta.transform.SetAsLastSibling();
        }

        // NOVO: Zoom no Canvas usando o Canvas Scaler
        if (usarAnimacaoZoom && canvasScaler != null)
        {
            Debug.Log("Iniciando a animação de zoom.");
            float t = 0;
            while (t < duracaoZoom)
            {
                t += Time.deltaTime;
                float progress = t / duracaoZoom;
                canvasScaler.scaleFactor = Mathf.Lerp(escalaOriginal, escalaOriginal * fatorZoom, progress);
                yield return null;
            }
        }

        // Anima a vinheta (fade in)
        if (vinheta != null)
        {
            float t = 0;
            Color cor = vinheta.color;
            while (t < duracaoVinheta)
            {
                t += Time.deltaTime;
                float alpha = Mathf.Lerp(0, 1, t / duracaoVinheta);
                vinheta.color = new Color(cor.r, cor.g, cor.b, alpha);
                yield return null;
            }
        }

        // Carrega a cena
        if (!string.IsNullOrEmpty(nomeCena))
        {
            SceneManager.LoadScene(nomeCena);
        }
    }
}