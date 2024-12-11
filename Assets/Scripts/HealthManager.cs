using System.Collections;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public int Starthealth = 24;
    public NumberStringDisplay numberStringDisplay;
    public Animator anim;

    public bool isDead = false; // Флаг состояния смерти

    void Start()
    {
        anim = GetComponent<Animator>();
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);
    }

    void Update()
    {

        // Пример уменьшения здоровья для теста
        if (Input.GetKeyDown(KeyCode.Y) && Starthealth > 0)
        {
            Starthealth -= 1;
            numberStringDisplay.SetDoubleDigitNumber(Starthealth);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            numberStringDisplay.SetKey(true);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            numberStringDisplay.SetKey(false);
        }
    }

    public void TakeDamage()
    {
        if (isDead) return; // Нельзя нанести урон, если персонаж уже мертв

        Starthealth -= 1;
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);

        StartCoroutine(numberStringDisplay.BlinkEffect());

        if (Starthealth <= 0)
        {
            Die();
        }
    }

    public void Kill()
    {
        if (isDead) return; // Уже мертв

        Starthealth = 0;
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);
        Die();
    }

    void Die()
    {
        anim.SetTrigger("DeathTrigger"); // Запускаем анимацию смерти
        isDead = true; // Устанавливаем флаг смерти
    }

    // Метод для Animation Event, если нужно добавить событие в анимации
    public void OnDeathAnimationComplete()
    {
        Debug.Log("Animation Event: Death animation finished.");
        gameObject.SetActive(false);
        // Можно добавить действия после смерти, например, перезапуск уровня
    }
}
