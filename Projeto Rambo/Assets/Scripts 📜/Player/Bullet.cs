using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 10f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // Checa se bateu em uma caixa
        CaixaTeste caixa = col.collider.GetComponent<CaixaTeste>();
        if (caixa != null)
        {
            caixa.DestruirCaixa(); // chama o método público
        }

        Destroy(gameObject); // destrói a bala sempre
    }
}