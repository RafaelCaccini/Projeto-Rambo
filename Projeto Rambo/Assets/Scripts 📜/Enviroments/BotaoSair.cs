using UnityEngine;

public class BotaoSair : MonoBehaviour
{
    public void Sair()
    {
        Debug.Log("Fechando o jogo...");
        Application.Quit();
    }
}
