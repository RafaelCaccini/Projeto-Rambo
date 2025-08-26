using UnityEngine;
using UnityEngine.SceneManagement;

public class BotaoTrocarCena : MonoBehaviour
{
    [Header("Nome da Cena para carregar")]
    public string nomeCena;

    // Este m�todo pode ser chamado pelo bot�o no OnClick
    public void CarregarCena()
    {
        if (!string.IsNullOrEmpty(nomeCena))
        {
            SceneManager.LoadScene(nomeCena);
        }
        else
        {
            Debug.LogWarning("Nenhum nome de cena foi definido no bot�o: " + gameObject.name);
        }
    }
}
