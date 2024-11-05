/*using UnityEngine;

public class LadderDetector : MonoBehaviour
{
    public LadderMovement playerLadderMovement; // Ссылка на компонент LadderMovement игрока

    void Start()
    {
        // Ищем компонент LadderMovement у родительского объекта
        playerLadderMovement = GetComponentInParent<LadderMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            playerLadderMovement.isOnLadder = true; // Устанавливаем флаг в LadderMovement
            playerLadderMovement.ladderCollider = collision.GetComponent<BoxCollider2D>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            playerLadderMovement.isOnLadder = false; // Сбрасываем флаг в LadderMovement
            playerLadderMovement.StopClimbing();     // Прекращаем подъем
        }
    }
}
*/