using UnityEngine;
using UnityEngine.InputSystem;

public class SeletorFases : MonoBehaviour
{
    public float velocidade = 5f;

    void Update()
    {
        Vector2 movimento = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) movimento.y += 1;
        if (Keyboard.current.sKey.isPressed) movimento.y -= 1;
        if (Keyboard.current.dKey.isPressed) movimento.x += 1;
        if (Keyboard.current.aKey.isPressed) movimento.x -= 1;

        movimento = movimento.normalized;

        transform.Translate(movimento * velocidade * Time.deltaTime);
    }
}