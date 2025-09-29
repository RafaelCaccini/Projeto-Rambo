using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // necessário para o novo Input System

public class TrocarFase : MonoBehaviour
{
    void Update()
    {
        var teclado = Keyboard.current; // pega o teclado

        // Se apertar 1, 2 ou 3, carrega a cena correspondente
        if (teclado.digit1Key.wasPressedThisFrame)
            CarregarFase("Level1");
        else if (teclado.digit2Key.wasPressedThisFrame)
            CarregarFase("Level2");
        else if (teclado.digit3Key.wasPressedThisFrame)
            CarregarFase("Level3");
    }

    // Função que carrega a cena
    private void CarregarFase(string nomeDaFase)
    {
        Debug.Log($"Tentando carregar cena: {nomeDaFase}"); // só pra ver no console
        SceneManager.LoadScene(nomeDaFase); // carrega a fase
    }
}
