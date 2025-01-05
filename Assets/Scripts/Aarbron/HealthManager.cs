using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour
{
    [Header("General Health")]
    public int Starthealth = 24;              // Количество здоровья
    public NumberStringDisplay numberStringDisplay; // Отображение здоровья
    public Animator anim;                     // Аниматор для проигрывания анимаций урона/смерти

    private PlatformerPlayer playerCode;       // Ссылка на скрипт персонажа
    private LadderMovement ladderCode;         // Ссылка на лазание по лестнице (если нужно)
    private CombatSystem combatSystem;         // Ссылка на систему атаки

    [Header("Invincibility Settings")]
    public bool isDead = false;               // Флаг состояния смерти
    public bool isInvincible = false;         // Флаг временной неуязвимости
    public float invincibilityDuration = 2f;  // Длительность неуязвимости

    [Header("Audio Settings")]
    public AudioClip[] damageSounds;          // Массив звуков для получения урона
    private AudioSource audioSource;          // Компонент для воспроизведения звуков

    public bool getdamage = false;            // Если нужно что-то проверять извне
    public bool button = false;
    void Start()
    {
        playerCode = GetComponent<PlatformerPlayer>();
        ladderCode = GetComponent<LadderMovement>();
        anim = GetComponent<Animator>();
        combatSystem = GetComponent<CombatSystem>();
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);
        audioSource = gameObject.AddComponent<AudioSource>(); // Инициализируем AudioSource
    }
    public void TakeDamageBoss()
    {
        if (isDead || isInvincible) return; // Не получаем урон, если мертвы или неуязвимы

        Starthealth -= 1;
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);

        // Если здоровье упало ниже 0 — смерть
        if (!isDead && Starthealth <= 0)
        {
            isDead = true;
            Die();
            playerCode.body.velocity = Vector2.zero;
        }
    }
    public void TakeDamage()
    {
        if (isDead || isInvincible) return; // Не получаем урон, если мертвы или неуязвимы

        Starthealth -= 1;
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);

        // Сбрасываем атаку, если во время получения урона персонаж атакует
        combatSystem.ResetAttack();
        combatSystem.isAttackingReverse = false;

        // Если здоровье упало ниже 0 — смерть
        if (!isDead && Starthealth <= 0)
        {
            isDead = true;
            Die();
            playerCode.body.velocity = Vector2.zero;
        }

        // Если все ещё живы
        if (Starthealth > 0)
        {
            // Эффект мерцания / индикации
            StartCoroutine(numberStringDisplay.BlinkEffect());

            // Включаем временную неуязвимость
            StartCoroutine(InvincibilityCoroutine());
        }

        // Сбросим флаги на случай, если персонаж был в особом состоянии
        if (playerCode.isTurning)
        {
            playerCode.isTurning = false;
            playerCode.previousDirection = transform.localScale.x;
            playerCode.isCrouching = false;
        }
        if (playerCode.isMovementLocked)
        {
            playerCode.isMovementLocked = false;
        }

        // Запускаем анимацию получения урона,
        // но только если не на лестнице, не в падении и не прыжке, и при этом живы
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
        }

        // Проиграть звук получения урона
        PlayDamageSound();
    }

    public void HPPotion()
    {
        Starthealth = 24;
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);
    }
    public void PlayDamageSound()
    {
        if (damageSounds.Length > 0)
        {
            int index = Random.Range(0, damageSounds.Length);
            audioSource.PlayOneShot(damageSounds[index]);
        }
    }

    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }
    public void Kill()
    {
        if (isDead) return;
        Starthealth = 0;
        isDead = true;
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);
        PlayDamageSound();
        Die();

    }

    public void Die()
    {
        anim.SetTrigger("DeathTrigger");
    }

    public void OnDeathAnimationComplete()
    {
        SceneManager.LoadScene("DeathScene"); // Переключение на сцену с видео
        gameObject.SetActive(false);
    }

    public void DamageReset()
    {
        combatSystem.ResetAttack();
        playerCode.isTurning = false;
    }
}
