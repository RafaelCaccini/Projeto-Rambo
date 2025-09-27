using UnityEngine;
using UnityEngine.SceneManagement;

public class BotaoTrocarCena : MonoBehaviour
{
    [Header("Nome da Cena para carregar")]
    public string nomeCena; // nome da cena que o bot�o vai abrir

    // M�todo que o bot�o chama quando clicado
    public void CarregarCena()
    {
        if (!string.IsNullOrEmpty(nomeCena))
        {
            // abre a cena definida
            SceneManager.LoadScene(nomeCena);
        }
        else
        {
            // avisa se n�o colocou o nome da cena
            Debug.LogWarning("Nenhum nome de cena foi definido no bot�o: " + gameObject.name);
        }
    }
}
