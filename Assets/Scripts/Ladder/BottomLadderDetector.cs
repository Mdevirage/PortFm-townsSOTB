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
            //.Log("�����");
            playerLadderMovement.isBottomDetectorActive = true; // ���������� ���� ��� �������� ����
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            //Debug.Log("����� �� ������ ���������");
            playerLadderMovement.isBottomDetectorActive = false; // ������������ ���� ��� �������� ����
        }
    }
}
