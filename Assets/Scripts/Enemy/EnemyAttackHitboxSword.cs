using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackHitboxSword : MonoBehaviour
{
    // Ссылка на основной скрипт врага (Sword), чтобы обновлять флаги
    private Sword parentSword;

    private void Awake()
    {
        // Получаем ссылку на родительский скрипт
        parentSword = GetComponentInParent<Sword>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Сообщаем, что игрок вошёл в зону удара
            parentSword.SetPlayerInAttackZone(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Сообщаем, что игрок вышел из зоны удара
            parentSword.SetPlayerInAttackZone(false);
        }
    }
}
