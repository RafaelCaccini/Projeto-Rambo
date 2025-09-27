using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 10f; // tempo que a bala vai existir

    void Start()
    {
        Destroy(gameObject, lifeTime); // destrói a bala depois do tempo definido
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // verifica se colidiu com uma caixa
        CaixaTeste caixa = col.collider.GetComponent<CaixaTeste>();
        if (caixa != null)
        {
            caixa.DestruirCaixa(); // faz a caixa gerar item e sumir
        }

        Destroy(gameObject); // destrói a bala sempre que colide
    }
}
