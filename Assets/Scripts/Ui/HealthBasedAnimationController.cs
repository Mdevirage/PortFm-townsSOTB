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

        if (healthManager.Starthealth <= 20)
        {
            // Ускоряем анимацию при низком уровне здоровья
            animator.speed = 1.125f; // Ускорение (настройте по желанию)
        }
        else if (healthManager.Starthealth <= 15)
        {
            // Возвращаем стандартную скорость анимации
            animator.speed = 1.25f; // Нормальная скорость
        }
        else if (healthManager.Starthealth <= 10)
        {
            animator.speed = 1.5f;
        }
        else if(healthManager.Starthealth <= 5)
        {
            animator.speed = 1.75f;
        }
    }
}
