using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float damage = 1f;
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col == null) return;

        if (col.CompareTag("Player"))
        {
            LifeScript vida = col.GetComponentInParent<LifeScript>();
            if (vida != null)
                vida.TomarDano((int)damage);

            Destroy(gameObject);
            return;
        }

        if (col.CompareTag("PlayerBullet"))
            return;

        LifeScript outraVida = col.GetComponentInParent<LifeScript>();
        if (outraVida != null)
            outraVida.TomarDano((int)damage);

        Destroy(gameObject);
    }
}
