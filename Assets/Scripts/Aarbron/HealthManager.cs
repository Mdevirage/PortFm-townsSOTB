using System.Collections;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public int Starthealth = 24; // Количество здоровья
    public NumberStringDisplay numberStringDisplay; // Отображение здоровья
    public Animator anim;
    private PlatformerPlayer playerCode;
    private LadderMovement ladderCode;
    private CombatSystem combatSystem;

    public bool isDead = false; // Флаг состояния смерти
    public bool isInvincible = false; // Флаг временной неуязвимости
    public float invincibilityDuration = 2f; // Длительность неуязвимости

    [Header("Audio Settings")]
    public AudioClip[] damageSounds; // Массив звуков для получения урона
    private AudioSource audioSource; // Компонент для воспроизведения звуков

    public bool getdamage =false;
    void Start()
    {
        playerCode = GetComponent<PlatformerPlayer>();
        ladderCode = GetComponent<LadderMovement>();
        anim = GetComponent<Animator>();
        combatSystem = GetComponent<CombatSystem>();
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);

        // Инициализируем AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
    }
    public void TakeDamage()
    {
        if (isDead || isInvincible) return; // Не получаем урон, если мертвы или неуязвимы

        
        Starthealth -= 1;
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);
        //getdamage = true;
        combatSystem.ResetAttack(); // Сбрасываем атаку при получении урона
        combatSystem.isAttackingReverse = false;
        if (!isDead && Starthealth - 1 < 0)
        {
            Die();
            isDead = true;
            playerCode.body.velocity = Vector2.zero;
        }
        if (Starthealth > 0)
        {
            StartCoroutine(numberStringDisplay.BlinkEffect());
            StartCoroutine(InvincibilityCoroutine()); // Запускаем временную неуязвимость
        }
        // Если персонаж поворачивается, сбросим это состояние
        if (playerCode.isTurning)
        {
            playerCode.isTurning = false;
            playerCode.previousDirection = transform.localScale.x;
            playerCode.isCrouching = false;
        }
        if (playerCode.isMovementLocked) { 
            playerCode.isMovementLocked = false;
        }
        // Запускаем анимацию получения урона
        if (!ladderCode.isClimbing && !playerCode.isFalling && !playerCode.isJumping && !isDead)
        {
            if (playerCode.isCrouching)
            {
                anim.SetTrigger("TakeDamageCrouching");
            }
            else
            {
                anim.SetTrigger("TakeDamageStanding");
            }

            PlayDamageSound(); // Воспроизводим звук урона
        }
    }

    private void PlayDamageSound()
    {
        if (damageSounds.Length > 0) // Проверяем, есть ли звуки в массиве
        {
            int index = Random.Range(0, damageSounds.Length); // Выбираем случайный звук
            audioSource.PlayOneShot(damageSounds[index]); // Проигрываем звук
        }
    }

    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true; // Включаем неуязвимость
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false; // Отключаем неуязвимость
    }

    public void Kill()
    {
        if (isDead) return;

        Starthealth = 0;
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);
        Die();
    }

    public void Die()
    {
        anim.SetTrigger("DeathTrigger"); // Запускаем анимацию смерти
    }

    public void OnDeathAnimationComplete()
    {
        //Debug.Log("Animation Event: Death animation finished.");
        gameObject.SetActive(false);
    }
    public void DamageReset() 
    {
        combatSystem.ResetAttack();
        playerCode.isTurning = false;

    }
}
