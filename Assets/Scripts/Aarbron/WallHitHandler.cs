using UnityEngine;

/// <summary>
/// Скрипт, объединяющий:
/// 1) Считывание нажатий (Input) в Update()
/// 2) Логику обнаружения "удара о стену" (OnCollisionEnter2D / OnCollisionStay2D)
/// 3) Одноразовые удары с блокировкой движения
/// 4) Пример "один удар за прыжок" (jumpWallHitUsed)
/// 5) Отнятие здоровья через HealthManager
/// 6) Логику "lockedDirection", чтобы разблокироваться при смене/отпускании нужной кнопки
/// </summary>
[RequireComponent(typeof(PlatformerPlayer))]
[RequireComponent(typeof(HealthManager))]
public class WallHitHandler : MonoBehaviour
{
    [Header("Wall Hit Settings")]
    public int wallHitThreshold = 5;      // Порог, при котором наносится урон
    private int wallHitCount;             // Текущий счётчик ударов

    // Если хотим только один удар за прыжок, используем этот флаг
    private bool jumpWallHitUsed = false;

    // Флаг: движение заблокировано из-за удара о стену
    private bool lockedByWallHit = false;

    // -1 = персонаж "прилип" к стене слева (нажимали левую кнопку)
    // +1 = к стене справа (нажимали правую)
    //  0 = нет блокировки или не определено
    private int lockedDirection = 0;
    // Ссылки на другие компоненты
    private PlatformerPlayer player;
    private HealthManager healthManager;

    // ------------------------------
    // Поля для "буфера ввода":
    // ------------------------------
    private bool pressLeftKeyDown;
    private bool pressRightKeyDown;
    private bool holdLeftKey;
    private bool holdRightKey;

    void Awake()
    {
        // Получаем ссылки на сопутствующие скрипты
        player = GetComponent<PlatformerPlayer>();
        healthManager = GetComponent<HealthManager>();
    }

    void Update()
    {
        // ========= 1) Считываем ввод (буфер ввода) =========

        // Одноразовые нажатия (работают ровно один кадр):
        pressLeftKeyDown = Input.GetKeyDown(KeyCode.LeftArrow);
        pressRightKeyDown = Input.GetKeyDown(KeyCode.RightArrow);

        // Удержание (работают, пока кнопка нажата):
        holdLeftKey = Input.GetKey(KeyCode.LeftArrow);
        holdRightKey = Input.GetKey(KeyCode.RightArrow);

        // ========= 2) Логика "один раз за прыжок" =========

        // Если персонаж приземлился и был в прыжке — сбрасываем флаг
        if (player.IsGrounded() && player.isJumping && jumpWallHitUsed)
        {
            player.isJumping = false;
            // Обнуляем анимацию (примерно так):
            healthManager.anim.SetBool("IsGrounded", true);
            // Звук приземления, если нужно
            player.landingSound.PlayLandingSound();
            jumpWallHitUsed = false;
        }

        // ========= 3) Разблокировка движения по отпусканию "той" кнопки =========
        if (lockedByWallHit)
        {
            // Если "прилипли" слева
            if (lockedDirection == -1)
            {
                // Пока держим левую кнопку, остаёмся заблокированными.
                // Как только отпустили её — разблокируемся.
                if (!holdLeftKey)
                {
                    lockedByWallHit = false;
                    lockedDirection = 0;
                    player.isMovementLocked = false;
                }
            }
            // Если "прилипли" справа
            else if (lockedDirection == +1)
            {
                if (!holdRightKey)
                {
                    lockedByWallHit = false;
                    lockedDirection = 0;
                    player.isMovementLocked = false;
                }
            }
            else
            {
                // Если lockedDirection == 0 (теоретически не должно быть),
                // но если вдруг, можно разблокировать:
                lockedByWallHit = false;
                player.isMovementLocked = false;
            }
        }
    }

    // Срабатывает при первом входе в коллизию со стеной
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Wall")) return;

        // Перебираем точки контакта
        foreach (ContactPoint2D contact in collision.contacts)
        {
            Vector2 normal = contact.normal;
            // Проверяем, что стена сбоку (почти горизонтальная нормаль)
            if (Mathf.Abs(normal.x) > 0.5f)
            {
                bool wallOnLeft = (normal.x > 0);
                bool wallOnRight = (normal.x < 0);

                // Держит ли игрок уже кнопку?
                bool isPushingWall =
                    (wallOnLeft && holdLeftKey) ||
                    (wallOnRight && holdRightKey);

                if (isPushingWall)
                {
                    HandleWallCollision();
                    break;
                }
            }
        }
    }

    // Срабатывает каждый кадр, пока персонаж остаётся в коллизии со стеной
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Wall")) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            Vector2 normal = contact.normal;
            if (Mathf.Abs(normal.x) > 0.5f)
            {
                bool wallOnLeft = (normal.x > 0);
                bool wallOnRight = (normal.x < 0);

                // Новое нажатие (одноразовое) при удерживании стены
                bool isPushingWall =
                    (wallOnLeft && pressLeftKeyDown) ||
                    (wallOnRight && pressRightKeyDown);

                if (isPushingWall)
                {
                    HandleWallCollision();
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Вызывается, если определили, что игрок "упирается" в стену
    /// (при входе или при новом нажатии).
    /// </summary>
    private void HandleWallCollision()
    {
        if (player.isJumping)
        {
            // Один удар за прыжок
            if (!jumpWallHitUsed)
            {
                jumpWallHitUsed = true;
                IncrementWallHit();
            }
        }
        else
        {
            IncrementWallHit();
        }
    }

    /// <summary>
    /// Увеличиваем счётчик "ударов о стену", наносим урон при достижении порога
    /// и блокируем движение до отпускания "той" кнопки, которая вызвала удар.
    /// </summary>
    private void IncrementWallHit()
    {
        wallHitCount++;
        Debug.Log($"WallHitCount = {wallHitCount}");

        // --- Определяем, какой стрелкой мы врезались --- 
        // Если holdLeftKey == true, значит удар слева; если holdRightKey == true, значит справа
        if (holdLeftKey && !holdRightKey)
        {
            lockedDirection = -1; // Врезались, удерживая левую
        }
        else if (holdRightKey && !holdLeftKey)
        {
            lockedDirection = +1; // Удерживали правую
        }
        else
        {
            // Если нажаты обе кнопки или что-то странное, можно взять
            // сторону стены из OnCollisionStay, но для примера упростим:
            lockedDirection = 0;
        }

        // Блокируем движение
        lockedByWallHit = true;
        player.isMovementLocked = true;

        // Если достигли порога — наносим урон
        if (wallHitCount >= wallHitThreshold)
        {
            wallHitCount = 0; // сбрасываем счётчик

            healthManager.Starthealth -= 1;
            healthManager.numberStringDisplay.SetDoubleDigitNumber(healthManager.Starthealth);

            // Если персонаж умер
            if (!healthManager.isDead && healthManager.Starthealth <= 0)
            {
                healthManager.Die();
                healthManager.isDead = true;
                player.body.velocity = Vector2.zero;
            }
            else
            {
                // Играем анимацию урона (если не прыгаем)
                if (!player.isJumping)
                {
                    if (player.isCrouching)
                        healthManager.anim.SetTrigger("TakeDamageCrouching");
                    else
                        healthManager.anim.SetTrigger("TakeDamageStanding");
                }
                healthManager.PlayDamageSound();
            }
        }
        else
        {
            // Если порог не достигнут — "лёгкая" анимация удара
            if (!player.isJumping)
            {
                if (player.isCrouching)
                    healthManager.anim.SetTrigger("TakeDamageCrouching");
                else
                    healthManager.anim.SetTrigger("TakeDamageStanding");
            }
            healthManager.PlayDamageSound();
        }
    }
}
