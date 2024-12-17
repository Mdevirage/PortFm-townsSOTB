using UnityEngine;

public class FireBallpoint : MonoBehaviour
{
    public GameObject FireProjectilePrefab; // ������ �������
    public Transform FireBallPoint;        // ����� ��������� �������
    private GameObject currentFireBall;    // ������ �� ��������� ������
    private bool playerInTrigger;          // ���� ���������� ������ � ��������

    private void Update()
    {
        if (playerInTrigger && currentFireBall == null) // ��������� ���������� ������ � ���������� Fire Ball
        {
            currentFireBall = Instantiate(FireProjectilePrefab, FireBallPoint.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // ���������, ��� � ������� ����� �����
        {
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // ���������, ��� ����� �������� �������
        {
            playerInTrigger = false;
        }
    }
}
