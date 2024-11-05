/*
using UnityEngine;

public class Ladder : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var ladderMovement = collision.GetComponent<LadderMovement>();
            if (ladderMovement != null)
            {
                ladderMovement.SetCurrentLadder(GetComponent<BoxCollider2D>());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var ladderMovement = collision.GetComponent<LadderMovement>();
            if (ladderMovement != null)
            {
                ladderMovement.ResetCurrentLadder();
            }
        }
    }
}
*/
