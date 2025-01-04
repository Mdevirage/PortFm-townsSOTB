using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackHitboxBug : MonoBehaviour
{
    // Ссылка на основной скрипт врага (Sword), чтобы обновлять флаги
    private BugEnemy parentBug;

    private void Awake()
    {
        // Получаем ссылку на родительский скрипт
        parentBug = GetComponentInParent<BugEnemy>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Сообщаем, что игрок вошёл в зону удара
            parentBug.SetPlayerInAttackZone(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Сообщаем, что игрок вышел из зоны удара
            parentBug.SetPlayerInAttackZone(false);
        }
    }
}
