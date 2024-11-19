using Cinemachine;
using UnityEngine;
using System.Collections;
using Unity.Mathematics;

public class LadderMovement : MonoBehaviour
{
    public float climbSpeed = 3.0f;
    public bool isClimbing = false;
    private bool isExitingClimb = false; // Флаг для блокировки движения при анимации выхода
    private bool CameraM = false;

    private Rigidbody2D body;
    private Animator anim;

    public bool isTopDetectorActive = false;
    public bool isBottomDetectorActive = false;
    public bool isOverlapLadderActive = false;

    public CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer framingTransposer;
    public GameObject tilemapToDisable;
    private int playerLayer;
    private int climbingPlayerLayer;
    private int groundLayer;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        playerLayer = LayerMask.NameToLayer("Player");
        climbingPlayerLayer = LayerMask.NameToLayer("ClimbingPlayer");
        groundLayer = LayerMask.NameToLayer("groundLayer");
    }

    void Update()
    {
        if (isClimbing)
        {
            if (isExitingClimb)
                return; // Блокируем управление при выходе

            if (CameraM)
            {
                framingTransposer.m_ScreenY = 0.806f;
                framingTransposer.m_SoftZoneHeight = 0f;

            }

            float verticalInput = Input.GetAxis("Vertical");

            if (isClimbing)
            {

                // Устанавливаем параметр ClimbSpeed для управления анимацией
                anim.SetFloat("ClimbSpeed", verticalInput);

                // Обновляем скорость перемещения персонажа
                body.velocity = new Vector2(0, verticalInput * climbSpeed);

                if (math.abs(verticalInput) <= 0.5)
                {
                    body.velocity = Vector2.zero; // Остановка персонажа при отсутствии ввода
                    anim.SetFloat("ClimbSpeed",0);
                }
            }

            if ((!isTopDetectorActive && isBottomDetectorActive && Input.GetKey(KeyCode.UpArrow))
                || (isTopDetectorActive && !isBottomDetectorActive && Input.GetKey(KeyCode.DownArrow))
                || isOverlapLadderActive && (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow)))
            {
                StopClimbing();
            }
        }
        else
        {
            if (body.velocity.y == 0 && isTopDetectorActive && (Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow)) && Input.GetAxis("Vertical") != 0)
            {
                StartClimbing(true); // Начинаем подъем
            }
            else if (body.velocity.y == 0 && isBottomDetectorActive && (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.UpArrow)) && Input.GetAxis("Vertical") != 0)
            {
                StartClimbing(false); // Начинаем спуск
            }
        }
    }
    float horizontalposition;
    float direction;
    public void StartClimbing(bool isClimbingUp)
    {
        isClimbing = true;
        isExitingClimb = true; // Снимаем флаг выхода
        gameObject.layer = climbingPlayerLayer;

        // Игнорируем коллизии между ClimbingPlayer и Ground
        Physics2D.IgnoreLayerCollision(climbingPlayerLayer, groundLayer, true);


        body.gravityScale = 0;
        body.velocity = Vector2.zero;

        // Находим центр ближайшей лестницы и определяем направление подхода
        BoxCollider2D closestLadder = FindClosestLadder();
        Vector2 ladderCenter = closestLadder.bounds.center;
        // Запускаем анимацию подъема или спуска
        if (isClimbingUp)
        {
            if (closestLadder != null)
            {
                horizontalposition = ladderCenter.x;
                anim.SetTrigger("StartClimbUp"); // Анимация для подъема
                transform.position = new Vector2(ladderCenter.x, transform.position.y);
            }
        }
        else
        {
            if (closestLadder != null)
            {

                horizontalposition = ladderCenter.x;
                direction = transform.position.x - ladderCenter.x; // Положительное - персонаж справа, отрицательное - слева
                if (direction > 0)
                {
                    // Персонаж подходит справа, выбираем анимацию спуска справа
                    anim.SetTrigger("StartClimbDownRight");
                }
                else
                {
                    // Персонаж подходит слева, выбираем анимацию спуска слева
                    anim.SetTrigger("StartClimbDownLeft");
                }

                // Выравниваем персонажа по центру лестницы (универсально для обоих случаев)
                transform.position = new Vector2(ladderCenter.x, transform.position.y);
            }
        }
        anim.SetBool("IsClimbing", true);
    }

    public void StopClimbing()
    {
        isExitingClimb = true;// Активируем флаг выхода
        Debug.Log("isClimbing False");
        gameObject.layer = playerLayer;

        // Восстанавливаем коллизии между ClimbingPlayer и Ground
        Physics2D.IgnoreLayerCollision(climbingPlayerLayer, groundLayer, false);

        body.gravityScale = 1;

        // Определяем направление выхода
        if (isTopDetectorActive)
        {
            anim.SetTrigger("ExitClimbUp");
        }
        else if (isBottomDetectorActive)
        {
            anim.SetTrigger("ExitClimbDown");
        }

        anim.SetBool("IsClimbing", false);
    }

    public void OnStartClimbDownAnimationComplete()
    {   
        transform.position = new Vector2(horizontalposition, transform.position.y-1.8f);
        framingTransposer.m_TrackedObjectOffset.y = -2.22f;
        framingTransposer.m_ScreenY = 0.806f;
        framingTransposer.m_SoftZoneHeight = 0f;
    }
    public void OnStartClimbUpAnimationComplete()
    {
        CameraM = true;
        transform.position = new Vector2(horizontalposition, transform.position.y);
    }

    public void OnExitClimbComplete()
    {
        // Вызывается по завершении анимации выхода (через событие анимации)
        isExitingClimb = false;
    }
    public void CamMoveD()
    {

        StartCoroutine(SmoothTrackedObjectOffset(new Vector2(0, -4.02f), 0.6f));
    }
    public void CamMoveU()
    {
        framingTransposer.m_SoftZoneHeight = 0.3f;
        StartCoroutine(SmoothCameraShift(new Vector2(0, 0.02f), 1f));
    }
    public void OnExitClimbAnimationComplete()
    {
        // Этот метод вызывается анимацией после её завершения
        isClimbing = false;
        isExitingClimb = false;
        CameraM = false;  // Сбросим флаг движения камеры, если требуется
        framingTransposer.m_ScreenY = 0.578f;
        framingTransposer.m_SoftZoneHeight = 0.5f;
    }
    public void OnExitClimbDownAnimationComplete()
    {
        // Этот метод вызывается анимацией после её завершения
        isClimbing = false;
        isExitingClimb = false;
        CameraM = false;  // Сбросим флаг движения камеры, если требуется
        framingTransposer.m_ScreenY = 0.578f;
        framingTransposer.m_SoftZoneHeight = 0.5f;
        framingTransposer.m_TrackedObjectOffset.y = -2.22f;
    }
    // Метод для поиска ближайшей лестницы
    private BoxCollider2D FindClosestLadder()
    {
        // Размеры прямоугольника поиска (ширина и высота)
        Vector2 boxSize = new Vector2(1.0f, 3.0f);

        // Смещение прямоугольника относительно позиции персонажа
        Vector2 boxOffset = new Vector2(0f, -1.75f);

        // Позиция центра прямоугольника
        Vector2 boxCenter = (Vector2)transform.position + boxOffset;

        // Найти все коллайдеры в области прямоугольника
        Collider2D[] colliders = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f);
        BoxCollider2D closestLadder = null;
        float closestDistance = Mathf.Infinity;

        foreach (var collider in colliders)
        {
            // Проверяем, является ли объект лестницей
            if (collider.CompareTag("Ladder") && collider is BoxCollider2D boxCollider)
            {
                float distance = Vector2.Distance(transform.position, boxCollider.bounds.center);
                if (distance < closestDistance)
                {
                    closestLadder = boxCollider;
                    closestDistance = distance;
                }
            }
        }

        return closestLadder;
    }
    private IEnumerator SmoothTrackedObjectOffset(Vector2 targetOffset, float duration)
    {
        Vector2 initialOffset = framingTransposer.m_TrackedObjectOffset; // Текущее смещение
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            // Плавное изменение Tracked Object Offset
            framingTransposer.m_TrackedObjectOffset = Vector2.Lerp(initialOffset, targetOffset, elapsedTime / duration);
            yield return null;
        }

        // Устанавливаем финальное значение
        framingTransposer.m_TrackedObjectOffset = targetOffset;
    }
    public void DisableTilemap()
    {
        if (tilemapToDisable != null)
        {
            tilemapToDisable.SetActive(false);
            Debug.Log($"Tilemap {tilemapToDisable.name} отключен.");
        }
        else
        {
            Debug.LogWarning("Tilemap для отключения не назначен.");
        }
    }
    private IEnumerator SmoothCameraShift(Vector2 targetOffset, float duration)
    {
        Vector2 initialOffset = framingTransposer.m_TrackedObjectOffset; // Текущее смещение
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Линейная интерполяция между текущим и целевым смещением
            framingTransposer.m_TrackedObjectOffset = Vector2.Lerp(initialOffset, targetOffset, elapsedTime / duration);
            yield return null;
        }

        // Устанавливаем финальное значение
        framingTransposer.m_TrackedObjectOffset = targetOffset;
    }
    public void EnableTilemap()
    {
        if (tilemapToDisable != null)
        {
            tilemapToDisable.SetActive(true);
            Debug.Log($"Tilemap {tilemapToDisable.name} включен.");
        }
        else
        {
            Debug.LogWarning("Tilemap для включения не назначен.");
        }
    }

    public void ClimbDownMovement(float offset)
    {
        transform.position = new Vector2(horizontalposition, transform.position.y + offset);
    }
}