using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ExplosionData
{
    public Transform explosionPoint;      // Точка, где будет происходить взрыв
    public GameObject explosionPrefab;    // Префаб взрыва, который будет инстанцирован
    public float delayAfterPrevious = 0f; // Задержка перед этим взрывом
}

public class BossTree : MonoBehaviour
{
    public int Health = 1;
    public bool isDead = false;

    [Header("Эффекты смерти")]
    public List<ExplosionData> explosions; // Список данных о взрывах

    [Header("Компоненты для анимации смерти")]
    public SpriteRenderer spriteRenderer1;
    public SpriteRenderer spriteRenderer2;
    public CombatSystem combatSystemplayer;
    public void TakeDamage()
    {
        if (isDead)
            return;

        Health -= 1;
        Debug.Log("Босс получил урон. Текущее здоровье: " + Health);

        if (!isDead && Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        isDead = true;
        // Запускаем корутину для последовательных взрывов
        StartCoroutine(TriggerDeathEffectsCoroutine());
        combatSystemplayer.canSpecialAttack = false;
        // Отключаем спрайты босса
        spriteRenderer1.enabled = false;
        spriteRenderer2.enabled = false;
        
    }

    IEnumerator TriggerDeathEffectsCoroutine()
    {
        foreach (ExplosionData explosion in explosions)
        {
            if (explosion.explosionPrefab != null && explosion.explosionPoint != null)
            {
                // Ждём задержку перед взрывом, если она задана
                if (explosion.delayAfterPrevious > 0f)
                {
                    yield return new WaitForSeconds(explosion.delayAfterPrevious);
                }

                // Инстанцируем префаб взрыва в каждой точке
                Instantiate(explosion.explosionPrefab, explosion.explosionPoint.position, Quaternion.identity);
                Debug.Log($"Взрыв создан в точке: {explosion.explosionPoint.position}");
            }
            else
            {
                Debug.LogWarning("BossTree: Префаб взрыва или точка не назначены.");
            }
        }

        // После всех взрывов уничтожаем объект босса
        Destroy(transform.parent.gameObject);
        Debug.Log("Босс уничтожен после всех взрывов.");
    }
}
