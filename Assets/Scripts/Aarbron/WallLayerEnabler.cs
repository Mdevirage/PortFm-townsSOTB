using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallLayerEnabler : MonoBehaviour
{
    private PlatformerPlayer player;
    void Start()
    {   
        player = GetComponentInParent<PlatformerPlayer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            if (player.isJumping)
            {
                player.EndJump();
            }
        }
    }
}
