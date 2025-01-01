using UnityEngine;

public class TeleportIn : MonoBehaviour
{
    public TeleportWithAnimations teleporter; // Ссылка на скрипт телепорта на игроке

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Просим у игрока запустить анимацию «исчезновения» 
            teleporter.StartTeleportSequence();
        }
    }
}
