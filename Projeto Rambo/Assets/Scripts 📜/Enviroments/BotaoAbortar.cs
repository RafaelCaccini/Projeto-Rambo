using UnityEngine;

public class BotaoAbortar : MonoBehaviour
{
    public void Sair()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();
    }
}
