using UnityEngine;

public class FireBallpoint : MonoBehaviour
{
    public GameObject FireProjectilePrefab; // Префаб снаряда
    public Transform FireBallPoint;        // Точка появления снаряда
    private GameObject currentFireBall;    // Ссылка на созданный объект
    private bool playerInTrigger;          // Флаг нахождения игрока в триггере

    private void Update()
    {
        if (playerInTrigger && currentFireBall == null) // Проверяем нахождение игрока и отсутствие Fire Ball
        {
            currentFireBall = Instantiate(FireProjectilePrefab, FireBallPoint.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Проверяем, что в триггер вошел игрок
        {
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Проверяем, что игрок покидает триггер
        {
            playerInTrigger = false;
        }
    }
}
