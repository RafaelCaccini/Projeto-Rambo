using UnityEngine;
using UnityEngine.SceneManagement;

public class BotaoTrocarCena : MonoBehaviour
{
    [Header("Nome da Cena para carregar")]
    public string nomeCena;

    // Este método pode ser chamado pelo botão no OnClick
    public void CarregarCena()
    {
        if (!string.IsNullOrEmpty(nomeCena))
        {
            SceneManager.LoadScene(nomeCena);
        }
        else
        {
            Debug.LogWarning("Nenhum nome de cena foi definido no botão: " + gameObject.name);
        }
    }
}
