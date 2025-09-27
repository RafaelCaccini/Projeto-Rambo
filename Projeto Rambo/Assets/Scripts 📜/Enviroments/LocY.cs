using UnityEngine;

public class LockY : MonoBehaviour
{
    private float yInicial;

    void Start()
    {
        // Guarda a posição Y inicial
        yInicial = transform.position.y;
    }

    void LateUpdate()
    {
        // Sempre mantém o Y na posição inicial
        transform.position = new Vector3(transform.position.x, yInicial, transform.position.z);
    }
}
