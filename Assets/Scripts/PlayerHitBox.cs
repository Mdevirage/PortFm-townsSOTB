using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, является ли объект врагом
        if (other.CompareTag("Enemy"))
        {
            SimpleEnemy enemy = other.GetComponent<SimpleEnemy>();
            Sphere sphere = other.GetComponent<Sphere>();
            if (enemy != null)
            {
                enemy.TakeDamage(); // Наносим урон врагу
                Debug.Log("Take damage");
            }
            if (sphere != null)
            {
                sphere.TakeDamage();
            }
        }
    }
}
