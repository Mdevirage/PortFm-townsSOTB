using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    private bool hasHit = false; // Флаг, указывающий, был ли уже нанесён урон
    private void OnEnable()
    {
        // Сбрасываем флаг при активации объекта
        hasHit = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasHit && other.CompareTag("Enemy"))
        {
            hasHit = true; // Устанавливаем флаг, чтобы предотвратить дальнейшие попадания

            // Получение компонентов врага
            BugEnemy bugEnemy = other.GetComponent<BugEnemy>();
            Sword sword = other.GetComponent<Sword>();
            Sphere sphere = other.GetComponent<Sphere>();
            Axe axe = other.GetComponent<Axe>();
            BossTree bossTree = other.GetComponent<BossTree>();
            Dragonfly dragonfly = other.GetComponent<Dragonfly>();
            Snake snake = other.GetComponent<Snake>();
            // Нанесение урона
            if (bugEnemy != null) bugEnemy.TakeDamage();
            if (sword != null) sword.TakeDamage();
            if (sphere != null) sphere.TakeDamage();
            if (axe != null) axe.TakeDamage();
            if (bossTree != null) bossTree.TakeDamage();
            if (dragonfly != null) dragonfly.TakeDamage();
            if (snake != null) snake.TakeDamage();
            // Опционально: отключение коллайдера после атаки
            // GetComponent<Collider2D>().enabled = false;
        }
    }
}
