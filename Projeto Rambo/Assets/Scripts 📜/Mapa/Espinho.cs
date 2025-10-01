using UnityEngine;

public class Spike : MonoBehaviour
{
    public int damage = 15;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LifeScript life = collision.GetComponent<LifeScript>();
            if (life != null)
            {
                life.TomarDano(damage);
            }
        }
        else if (collision.CompareTag("Boss"))
        {
            // ignora colisão com o boss
            Physics2D.IgnoreCollision(collision, GetComponent<Collider2D>());
        }
    }
}
