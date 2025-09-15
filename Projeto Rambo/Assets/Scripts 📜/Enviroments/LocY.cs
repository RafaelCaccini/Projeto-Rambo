using UnityEngine;

public class LockY : MonoBehaviour
{
    private float yInicial;

    void Start()
    {
        // Pega a posição inicial no Y
        yInicial = transform.position.y;
    }

    void LateUpdate()
    {
        // Mantém o Y travado
        transform.position = new Vector3(transform.position.x, yInicial, transform.position.z);
    }
}


