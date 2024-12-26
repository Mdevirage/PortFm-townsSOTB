using System.Collections;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [Header("General Health")]
    public int Starthealth = 24;              // Количество здоровья
    public NumberStringDisplay numberStringDisplay; // Отображение здоровья
    public Animator anim;                     // Аниматор для проигрывания анимаций урона/смерти

    private PlatformerPlayer playerCode;       // Ссылка на скрипт персонажа
    private LadderMovement ladderCode;         // Ссылка на лазание по лестнице (если нужно)
    private CombatSystem combatSystem;         // Ссылка на систему атаки

    [Header("Wall Hit Settings")]
    public int wallHitCount = 0;              // Текущее число столкновений со стеной
    public int wallHitThreshold = 5;          // Порог, при котором персонаж получит реальный урон
    public float wallHitCooldown = 0.5f;      // Кулдаун для учёта повторных ударов об стену
    private float lastWallHitTime;            // Когда последний раз засчитывали удар?
    public bool hasJumpCollision = false;

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
    void OnCollisionStay2D(Collision2D collision)
    {
        // Проверяем, является ли объект действительно «стеной»
        if (!collision.gameObject.CompareTag("Wall"))
            return;

        // Проверяем, находится ли персонаж в прыжке
        if (playerCode.isJumping && hasJumpCollision)
        {
            return; // Если уже было засчитано столкновение, не увеличиваем счётчик
        }

        foreach (ContactPoint2D contact in collision.contacts)
        {
            Vector2 normal = contact.normal;

            // Убедимся, что это почти горизонтальное столкновение:
            if (Mathf.Abs(normal.x) > 0.5f)
            {
                bool wallOnLeft = (normal.x > 0f);
                bool wallOnRight = (normal.x < 0f);

                if (IsPressingForwardIntoWall(wallOnLeft, wallOnRight))
                {
                    AddWallHit();

                    if (playerCode.isJumping)
                    {
                        hasJumpCollision = true; // Устанавливаем флаг столкновения в прыжке
                    }

                    break;
                }
            }
        }
    }

    /// <summary>
    /// Возвращает true, если игрок жмёт кнопку "вперёд" ровно в ту сторону, где стена.
    /// Допустим, если стена справа, а игрок нажал Horizontal > 0,
    /// это значит «давим вперёд».
    /// </summary>
    bool IsPressingForwardIntoWall(bool wallOnLeft, bool wallOnRight)
    {
        float inputX = 0;
        
        // Проверяем, нажал ли пользователь кнопку в этом кадре:
        if (Input.GetKey(KeyCode.LeftArrow) && !button)
        {
            inputX = -1;
            // Логика «одноразового» нажатия влево
        }
        else if (Input.GetKey(KeyCode.RightArrow) && !button)
        {
            inputX = 1;
            // Логика «одноразового» нажатия вправо
        }
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            button = false;
        }

        // Если стена слева, значит "вперёд" = inputX < 0
        // Если стена справа, значит "вперёд" = inputX > 0
        if (wallOnRight && inputX > 0) 
        {
            button = true;
            return true;
        }

        if (wallOnLeft && inputX < 0)
        {
            button = true;
            return true;
        }
        return false;
    }
    public void AddWallHit()
    {
        // Проверяем кулдаун
        if (Time.time - lastWallHitTime < wallHitCooldown)
            return;
        lastWallHitTime = Time.time;

        // Увеличиваем счётчик
        wallHitCount++;
        Debug.Log($"WallHitCount = {wallHitCount}");

        // Если достигнут порог — реальный урон
        if (wallHitCount >= wallHitThreshold)
        {
            wallHitCount = 0; // сбрасываем счётчик
            Starthealth -= 1;
            numberStringDisplay.SetDoubleDigitNumber(Starthealth);

            if (!isDead && Starthealth <= 0)
            {
                Die();
                isDead = true;
                playerCode.body.velocity = Vector2.zero;
            }
        }

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
        PlayDamageSound();
    }

    /// <summary>
    /// Реальный урон (минус 1 здоровье). Логика из вашего кода.
    /// </summary>
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
            Die();
            isDead = true;
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

    private void PlayDamageSound()
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
        gameObject.SetActive(false);
    }

    public void DamageReset()
    {
        combatSystem.ResetAttack();
        playerCode.isTurning = false;
    }
}
