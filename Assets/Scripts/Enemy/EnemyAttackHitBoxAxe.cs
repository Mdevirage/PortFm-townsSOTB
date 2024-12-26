using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackHitBoxAxe : MonoBehaviour
{
    private Axe parentAxe;
    // Start is called before the first frame update
    private void Awake()
    {
        // Получаем ссылку на родительский скрипт
        parentAxe = GetComponentInParent<Axe>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Сообщаем, что игрок вошёл в зону удара
            parentAxe.SetPlayerInAttackZone(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Сообщаем, что игрок вышел из зоны удара
            parentAxe.SetPlayerInAttackZone(false);
        }
    }
}
