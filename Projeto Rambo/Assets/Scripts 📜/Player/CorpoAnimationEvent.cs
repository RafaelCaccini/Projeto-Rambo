using UnityEngine;

public class CorpoAnimationEvents : MonoBehaviour
{
    private PlayerController player;

    void Awake()
    {
        player = GetComponentInParent<PlayerController>();
    }

    // Esse método aparecerá no Animation Event
    public void SpawnGranada()
    {
        if (player != null)
        {
            player.SpawnGranada();
        }
    }
}


