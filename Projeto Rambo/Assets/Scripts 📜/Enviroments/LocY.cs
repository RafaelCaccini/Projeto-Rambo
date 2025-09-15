using UnityEngine;

public class LockY : MonoBehaviour
{
    private float yInicial;

    void Start()
    {
        // Pega a posi��o inicial no Y
        yInicial = transform.position.y;
    }

    void LateUpdate()
    {
        // Mant�m o Y travado
        transform.position = new Vector3(transform.position.x, yInicial, transform.position.z);
    }
}


