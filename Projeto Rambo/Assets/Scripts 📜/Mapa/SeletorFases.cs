using UnityEngine;
using UnityEngine.InputSystem;

public class SeletorFases : MonoBehaviour
{
    public float velocidade = 5f; // velocidade do movimento do seletor

    void Update()
    {
        Vector2 movimento = Vector2.zero; // guarda a dire��o que vamos mover

        // verifica as teclas e ajusta a dire��o
        if (Keyboard.current.wKey.isPressed) movimento.y += 1; // cima
        if (Keyboard.current.sKey.isPressed) movimento.y -= 1; // baixo
        if (Keyboard.current.dKey.isPressed) movimento.x += 1; // direita
        if (Keyboard.current.aKey.isPressed) movimento.x -= 1; // esquerda

        // normaliza para n�o ficar mais r�pido na diagonal
        movimento = movimento.normalized;

        // move o objeto na dire��o calculada
        transform.Translate(movimento * velocidade * Time.deltaTime);
    }
}
