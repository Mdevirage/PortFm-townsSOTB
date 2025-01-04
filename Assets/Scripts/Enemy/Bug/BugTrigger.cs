using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugTrigger : MonoBehaviour
{
    public GameObject border1;
    public GameObject border2;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlatformerPlayer player = collision.gameObject.GetComponent<PlatformerPlayer>();
            if (player != null)
            {
                player.isMoveTrigger = true;
                border1.gameObject.SetActive(false);
                border2.gameObject.SetActive(false);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlatformerPlayer player = collision.gameObject.GetComponent<PlatformerPlayer>();
            if (player != null)
            {
                player.isMoveTrigger = false;
                border1.gameObject.SetActive(true);
                border2.gameObject.SetActive(true);
            }
        }
    }

}
