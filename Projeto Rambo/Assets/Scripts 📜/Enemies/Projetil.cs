using UnityEngine;

public class Projetil : MonoBehaviour
{
    public float tempoDeVida = 6f;
    public int dano = 20;

    private Collider2D meuCollider;

    void Start()
    {
        Destroy(gameObject, tempoDeVida);

        //==== IGNORAR COLISÕES COM OUTROS PROJÉTEIS DA MESMA LAYER ====
        meuCollider = GetComponent<Collider2D>();
        int minhaLayer = gameObject.layer;

        GameObject[] todos = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in todos)
        {
            if (obj == null || obj == this.gameObject) continue;

            // mesmo layer = ignorar
            if (obj.layer == minhaLayer)
            {
                Collider2D col = obj.GetComponent<Collider2D>();
                if (col != null)
                {
                    Physics2D.IgnoreCollision(meuCollider, col, true);
                }
            }
        }
        //===============================================================
    }

    void OnCollisionEnter2D(Collision2D colisao)
    {
        GameObject other = colisao.gameObject;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player levou " + dano + " de dano");
            Destroy(gameObject);
            return;
        }

        if (!other.CompareTag("Boss") && !other.CompareTag("Untagged"))
        {
            Destroy(gameObject);
        }
    }
}
