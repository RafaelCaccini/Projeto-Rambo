using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // necessário para o novo Input System

public class TrocarFase : MonoBehaviour
{
    void Update()
    {
        var teclado = Keyboard.current;

        if (teclado.digit1Key.wasPressedThisFrame)
            CarregarFase("Level1");
        else if (teclado.digit2Key.wasPressedThisFrame)
            CarregarFase("Level2");
        else if (teclado.digit3Key.wasPressedThisFrame)
            CarregarFase("Level3");
    }

    private void CarregarFase(string nomeDaFase)
    {
        Debug.Log($"Tentando carregar cena: {nomeDaFase}");
        SceneManager.LoadScene(nomeDaFase);
    }
}
