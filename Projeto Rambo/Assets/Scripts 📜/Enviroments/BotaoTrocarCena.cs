using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class BotaoTrocarCena : MonoBehaviour
{
    [Header("Nome da Cena para carregar")]
    public string nomeCena;

    [Header("Configuração de Animação do Botão")]
    public Image imagemDoBotao;
    public float duracaoFadeBotao = 0.5f;

    [Header("Configuração de Animação do Zoom")]
    public bool usarAnimacaoZoom = true;
    public CanvasScaler canvasScaler;  
    public float duracaoZoom = 0.5f;
    public float fatorZoom = 1.2f;

    [Header("Vinheta")]
    public Image vinheta;
    public float duracaoVinheta = 0.8f;

    private float escalaOriginal; 
    private bool jaCliquei = false;

    
    [Header("Fade de Áudio")]
    public AudioSource audioSourceParaDiminuir;
    public float duracaoFadeAudio = 1f;
 

    void Start()
    {
        if (imagemDoBotao != null)
            imagemDoBotao.color = new Color(imagemDoBotao.color.r, imagemDoBotao.color.g, imagemDoBotao.color.b, 0);

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

        // NOVO: começa a reduzir o volume junto com o clique
        if (audioSourceParaDiminuir != null)
            StartCoroutine(FadeAudio());

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

        if (!string.IsNullOrEmpty(nomeCena))
        {
            SceneManager.LoadScene(nomeCena);
        }
    }

    
    private IEnumerator FadeAudio()
    {
        float volumeInicial = audioSourceParaDiminuir.volume;
        float t = 0;

        while (t < duracaoFadeAudio)
        {
            t += Time.deltaTime;
            float progresso = t / duracaoFadeAudio;
            audioSourceParaDiminuir.volume = Mathf.Lerp(volumeInicial, 0, progresso);
            yield return null;
        }

        audioSourceParaDiminuir.volume = 0;
    }
  
}
