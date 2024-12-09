using System.Collections;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public int Starthealth = 24;
    public NumberStringDisplay numberStringDisplay;
    public Animator anim;

    private bool isDead = false; // Флаг состояния смерти

    void Start()
    {
        anim = GetComponent<Animator>();
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);
    }

    void Update()
    {
        if (isDead) return; // Если персонаж мертв, блокируем ввод

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
        isDead = true; // Устанавливаем флаг смерти
        anim.SetTrigger("DeathTrigger"); // Запускаем анимацию смерти
        StartCoroutine(HandleDeath());
    }

    // Корутину для блокировки действий во время проигрывания анимации
    IEnumerator HandleDeath()
    {
        // Ожидаем окончания анимации смерти
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        // Дополнительные действия после смерти
        Debug.Log("Death animation completed. Character is dead.");
    }

    // Метод для Animation Event, если нужно добавить событие в анимации
    public void OnDeathAnimationComplete()
    {
        Debug.Log("Animation Event: Death animation finished.");
        gameObject.SetActive(false);
        // Можно добавить действия после смерти, например, перезапуск уровня
    }
}
