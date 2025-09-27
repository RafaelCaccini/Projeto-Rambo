using UnityEngine;
using UnityEngine.SceneManagement;

public class BotaoTrocarCena : MonoBehaviour
{
    [Header("Nome da Cena para carregar")]
    public string nomeCena; // nome da cena que o botão vai abrir

    // Método que o botão chama quando clicado
    public void CarregarCena()
    {
        if (!string.IsNullOrEmpty(nomeCena))
        {
            // abre a cena definida
            SceneManager.LoadScene(nomeCena);
        }
        else
        {
            // avisa se não colocou o nome da cena
            Debug.LogWarning("Nenhum nome de cena foi definido no botão: " + gameObject.name);
        }
    }
}
