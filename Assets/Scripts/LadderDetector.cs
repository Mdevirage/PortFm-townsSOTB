/*using UnityEngine;

public class LadderDetector : MonoBehaviour
{
    public LadderMovement playerLadderMovement; // ������ �� ��������� LadderMovement ������

    void Start()
    {
        // ���� ��������� LadderMovement � ������������� �������
        playerLadderMovement = GetComponentInParent<LadderMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            playerLadderMovement.isOnLadder = true; // ������������� ���� � LadderMovement
            playerLadderMovement.ladderCollider = collision.GetComponent<BoxCollider2D>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            playerLadderMovement.isOnLadder = false; // ���������� ���� � LadderMovement
            playerLadderMovement.StopClimbing();     // ���������� ������
        }
    }
}
*/