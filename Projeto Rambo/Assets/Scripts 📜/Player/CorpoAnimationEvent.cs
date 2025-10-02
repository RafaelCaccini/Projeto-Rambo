using UnityEngine;

public class CorpoAnimationEvents : MonoBehaviour
{
    private PlayerController player;

    void Awake()
    {
        player = GetComponentInParent<PlayerController>();
    }

    // Esse m�todo aparecer� no Animation Event
    public void SpawnGranada()
    {
        if (player != null)
        {
            player.SpawnGranada();
        }
    }
}


