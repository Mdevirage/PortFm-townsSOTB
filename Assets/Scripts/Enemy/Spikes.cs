using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlatformerPlayer player = other.GetComponent<PlatformerPlayer>();
            if (player.isFalling) 
            {
                KillPlayer(other); // Pass the Collider2D to KillPlayer
            }
            
        }
    }

    private void KillPlayer(Collider2D playerCollider)
    {
        HealthManager playerHealth = playerCollider.GetComponent<HealthManager>();
        if (playerHealth != null)
        {
            playerHealth.Kill();
        }
    }
}
