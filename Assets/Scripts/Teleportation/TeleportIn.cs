using UnityEngine;

public class TeleportIn : MonoBehaviour
{
    public TeleportWithAnimations teleporter; // ������ �� ������ ��������� �� ������

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ������ � ������ ��������� �������� �������������� 
            teleporter.StartTeleportSequence();
        }
    }
}
