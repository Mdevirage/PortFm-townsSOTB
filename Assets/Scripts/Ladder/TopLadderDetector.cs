using UnityEngine;

public class TopLadderDetector : MonoBehaviour
{
    public LadderMovement playerLadderMovement;

    void Start()
    {
        playerLadderMovement = GetComponentInParent<LadderMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            //Debug.Log("Вход в верхний детектор");
            playerLadderMovement.isTopDetectorActive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            //Debug.Log("Выход из верхнего детектора");
            playerLadderMovement.isTopDetectorActive = false;
        }
    }
}
