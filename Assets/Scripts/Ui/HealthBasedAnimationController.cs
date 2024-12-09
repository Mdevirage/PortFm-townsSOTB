using UnityEngine;

public class HealthBasedAnimationController : MonoBehaviour
{
    public Animator animator;           // Ссылка на Animator
    public HealthManager healthManager; // Ссылка на HealthManager
    void Update()
    {
        if (healthManager.Starthealth <= 0)
        {
            // Останавливаем анимацию при смерти
            animator.speed = 0f;
            return;
        }

        if (healthManager.Starthealth <= 5)
        {
            // Ускоряем анимацию при низком уровне здоровья
            animator.speed = 1.5f; // Ускорение (настройте по желанию)
        }
        else
        {
            // Возвращаем стандартную скорость анимации
            animator.speed = 1f; // Нормальная скорость
        }
    }
}
