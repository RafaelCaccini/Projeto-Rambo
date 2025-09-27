using UnityEngine;

public class LockY : MonoBehaviour
{
    private float yInicial;

    void Start()
    {
        // Guarda a posi��o Y inicial
        yInicial = transform.position.y;
    }

    void LateUpdate()
    {
        // Sempre mant�m o Y na posi��o inicial
        transform.position = new Vector3(transform.position.x, yInicial, transform.position.z);
    }
}
