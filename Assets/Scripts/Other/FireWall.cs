using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWall : MonoBehaviour
{
    private bool isDamaging = false; // Флаг для предотвращения многократного запуска корутины
    public float damageCooldown = 0.5f; // Кулдаун в секундах между уронами
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isDamaging)
        {
            StartCoroutine(DealDamageOverTime(other));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isDamaging)
        {
            isDamaging = false;
            StopAllCoroutines(); // Останавливаем все корутины, если игрок выходит из зоны
        }
    }

    private IEnumerator DealDamageOverTime(Collider2D player)
    {
        isDamaging = true;

        while (isDamaging)
        {
            HealthManager playerHealth = player.GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(); // Наносим урон
            }

            yield return new WaitForSeconds(damageCooldown); // Ждём перед следующим нанесением урона
        }
    }
}
