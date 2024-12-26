using UnityEngine;

public class BottomLadderDetector : MonoBehaviour
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
            //
            //.Log("Внизу");
            playerLadderMovement.isBottomDetectorActive = true; // Активируем флаг для движения вниз
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            //Debug.Log("Выход из нижней детектора");
            playerLadderMovement.isBottomDetectorActive = false; // Деактивируем флаг для движения вниз
        }
    }
}
