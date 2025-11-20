using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 10f;
    public GameObject danoPrefab;

    private Collider2D meuCollider;

    void Start()
    {
        Destroy(gameObject, lifeTime);

        //==== IGNORAR OUTRAS BALAS DA MESMA LAYER ====
        meuCollider = GetComponent<Collider2D>();
        int minhaLayer = gameObject.layer;

        GameObject[] todos = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in todos)
        {
            if (obj == null || obj == this.gameObject) continue;

            if (obj.layer == minhaLayer)
            {
                Collider2D col = obj.GetComponent<Collider2D>();
                if (col != null)
                {
                    Physics2D.IgnoreCollision(meuCollider, col, true);
                }
            }
        }
        //================================================
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        CaixaTeste caixa = col.collider.GetComponent<CaixaTeste>();
        if (caixa != null)
        {
            caixa.DestruirCaixa();
        }

        Destroy(gameObject);
    }
}
