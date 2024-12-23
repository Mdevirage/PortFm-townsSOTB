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
            //Debug.Log("���� � ������� ��������");
            playerLadderMovement.isTopDetectorActive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            //Debug.Log("����� �� �������� ���������");
            playerLadderMovement.isTopDetectorActive = false;
        }
    }
}
