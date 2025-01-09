using UnityEngine;

[RequireComponent(typeof(PlatformerPlayer))]
[RequireComponent(typeof(HealthManager))]
public class WallHitHandler : MonoBehaviour
{
    [Header("Wall Hit Settings")]
    public int wallHitThreshold = 5;      // Порог для применения урона
    private int wallHitCount;             // Текущий счётчик ударов

    // Если требуется только один удар за прыжок, используем этот флаг
    private bool jumpWallHitUsed = false;

    // Флаг: движение заблокировано из-за удара о стену
    private bool lockedByWallHit = false;

    // -1 = персонаж "прилип" к левой стене (удерживает левую кнопку)
    // +1 = к правой стене (удерживает правую)
    //  0 = нет блокировки или не определено
    private int lockedDirection = 0;

    // Ссылки на другие компоненты
    private PlatformerPlayer player;
    private HealthManager healthManager;

    // Поля для "буфера ввода"
    private bool pressLeftKeyDown;
    private bool pressRightKeyDown;
    private bool holdLeftKey;
    private bool holdRightKey;

    [Header("Wall Detection Settings")]
    public float wallCheckDistance = 0.1f; // Дистанция для проверки стен
    public LayerMask wallLayerMask;        // Маска слоя для идентификации стен
    public Vector2 wallCheckOffsetLeft = new Vector2(-0.5f, 0f);  // Смещение для левостороннего raycast
    public Vector2 wallCheckOffsetRight = new Vector2(0.5f, 0f);  // Смещение для правостороннего raycast

    // Добавленные флаги для отслеживания обработанных столкновений
    private bool leftWallHitProcessed = false;
    private bool rightWallHitProcessed = false;

    void Awake()
    {
        // Получаем ссылки на сопутствующие скрипты
        player = GetComponent<PlatformerPlayer>();
        healthManager = GetComponent<HealthManager>();
    }

    void FixedUpdate()
    {
        // ========= 1) Считываем ввод (Буфер ввода) =========
        // Используем Input в Update(), но поскольку физические обновления в FixedUpdate, можно кэшировать ввод

        // Одноразовые нажатия
        pressLeftKeyDown = Input.GetButtonDown("Left");
        pressRightKeyDown = Input.GetButtonDown("Right");

        // Удержание кнопок
        holdLeftKey = Input.GetButton("Left");
        holdRightKey = Input.GetButton("Right");

        // ========= 2) Обработка Сброса Прыжка =========
        if (player.IsGrounded() && player.isJumping && jumpWallHitUsed)
        {
            player.isJumping = false;
            healthManager.anim.SetBool("IsGrounded", true);
            player.landingSound.PlayLandingSound();
            jumpWallHitUsed = false;
        }

        // ========= 3) Разблокировка Движения, если Необходимо =========
        HandleMovementUnlock();

        // ========= 4) Постоянное Обнаружение Стен =========
        DetectWalls();
    }

    /// <summary>
    /// Обнаруживает стены с обеих сторон с помощью raycasts.
    /// </summary>
    private void DetectWalls()
    {
        // Определяем исходные точки для raycasts на основе позиции игрока
        Vector2 originLeft = (Vector2)transform.position + wallCheckOffsetLeft;
        Vector2 originRight = (Vector2)transform.position + wallCheckOffsetRight;

        // Выполняем raycasts влево и вправо
        RaycastHit2D hitLeft = Physics2D.Raycast(originLeft, Vector2.left, wallCheckDistance, wallLayerMask);
        RaycastHit2D hitRight = Physics2D.Raycast(originRight, Vector2.right, wallCheckDistance, wallLayerMask);

        // Отладочные лучи (опционально, для визуализации)
        Debug.DrawRay(originLeft, Vector2.left * wallCheckDistance, Color.red);
        Debug.DrawRay(originRight, Vector2.right * wallCheckDistance, Color.blue);

        bool wallOnLeft = hitLeft.collider != null;
        bool wallOnRight = hitRight.collider != null;

        // Проверяем, удерживает ли игрок кнопку против стены и не было ли уже обработано столкновение
        bool isPushingWallLeft = wallOnLeft && holdLeftKey && !leftWallHitProcessed;
        bool isPushingWallRight = wallOnRight && holdRightKey && !rightWallHitProcessed;

        if (isPushingWallLeft || isPushingWallRight)
        {
            if (isPushingWallLeft)
            {
                HandleWallCollision(-1);
                leftWallHitProcessed = true; // Устанавливаем флаг
            }
            if (isPushingWallRight)
            {
                HandleWallCollision(+1);
                rightWallHitProcessed = true; // Устанавливаем флаг
            }
        }
    }

    /// <summary>
    /// Обрабатывает логику столкновения со стеной на основе направления.
    /// </summary>
    /// <param name="direction">-1 для левой, +1 для правой стороны</param>
    private void HandleWallCollision(int direction)
    {
        if (player.isJumping)
        {
            // Один удар за прыжок
            if (!jumpWallHitUsed)
            {
                jumpWallHitUsed = true;
                IncrementWallHit(direction);
            }
        }
        else
        {
            IncrementWallHit(direction);
        }
    }

    /// <summary>
    /// Увеличивает счётчик ударов о стену, наносит урон при достижении порога и блокирует движение.
    /// </summary>
    /// <param name="direction">Направление удара о стену (-1 или +1)</param>
    private void IncrementWallHit(int direction)
    {
        wallHitCount++;
        Debug.Log($"WallHitCount = {wallHitCount}");
        player.EndJump();
        // Блокируем движение
        lockedByWallHit = true;
        player.isMovementLocked = true;

        // Определяем направление блокировки на основе ввода
        lockedDirection = direction;

        if (player.isTurning)
        {
            player.isTurning = false;
            player.previousDirection = transform.localScale.x;
            player.isCrouching = false;
        }

        // Применяем урон, если достигнут порог
        if (wallHitCount >= wallHitThreshold)
        {
            wallHitCount = 0; // Сбрасываем счётчик

            healthManager.Starthealth -= 1;
            healthManager.numberStringDisplay.SetDoubleDigitNumber(healthManager.Starthealth);

            // Проверяем, умер ли персонаж
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
            // Играем лёгкую анимацию удара
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

    /// <summary>
    /// Разблокирует движение игрока, когда соответствующая кнопка отпущена.
    /// </summary>
    private void HandleMovementUnlock()
    {
        if (lockedByWallHit)
        {
            if (lockedDirection == -1 && !holdLeftKey)
            {
                UnlockMovement();
                leftWallHitProcessed = false; // Сбрасываем флаг для левой стороны
            }
            else if (lockedDirection == +1 && !holdRightKey)
            {
                UnlockMovement();
                rightWallHitProcessed = false; // Сбрасываем флаг для правой стороны
            }
            else if (lockedDirection == 0)
            {
                // Граничный случай: разблокировать, если направление неопределено
                UnlockMovement();
                leftWallHitProcessed = false;
                rightWallHitProcessed = false;
            }
        }
    }

    /// <summary>
    /// Разблокирует движение игрока.
    /// </summary>
    private void UnlockMovement()
    {
        lockedByWallHit = false;
        lockedDirection = 0;
        player.isMovementLocked = false;
    }

    void Update()
    {
        // Обработка ввода, который необходимо считывать каждый кадр
        // При необходимости можно переместить обработку ввода сюда вместо FixedUpdate
    }
}
