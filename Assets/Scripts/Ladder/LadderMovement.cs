using Cinemachine;
using UnityEngine;

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
            body.velocity = new Vector2(0, verticalInput * climbSpeed);

            if (verticalInput == 0)
                body.velocity = Vector2.zero;

            if ((!isTopDetectorActive && isBottomDetectorActive && Input.GetKey(KeyCode.UpArrow))
                || (isTopDetectorActive && !isBottomDetectorActive && Input.GetKey(KeyCode.DownArrow))
                || isOverlapLadderActive && (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow)))
            {
                StopClimbing();
            }
        }
        else
        {
            if (body.velocity.y == 0 && isTopDetectorActive && Input.GetKey(KeyCode.UpArrow) && Input.GetAxis("Vertical") != 0)
            {
                StartClimbing(true); // Начинаем подъем
            }
            else if (body.velocity.y == 0 && isBottomDetectorActive && Input.GetKey(KeyCode.DownArrow) && Input.GetAxis("Vertical") != 0)
            {
                StartClimbing(false); // Начинаем спуск
            }
        }
    }

    public void StartClimbing(bool isClimbingUp)
    {
        isClimbing = true;
        isExitingClimb = true; // Снимаем флаг выхода
        gameObject.layer = climbingPlayerLayer;

        // Игнорируем коллизии между ClimbingPlayer и Ground
        Physics2D.IgnoreLayerCollision(climbingPlayerLayer, groundLayer, true);

        
        body.gravityScale = 0;
        body.velocity = Vector2.zero;

        // Запускаем анимацию подъема или спуска
        if (isClimbingUp)
        {
            anim.SetTrigger("StartClimbUp");
        }
        else
        {
            anim.SetTrigger("StartClimbDown");
        }

        anim.SetBool("IsClimbing", true);

        // Находим центр ближайшей лестницы и выравниваем персонажа
        BoxCollider2D closestLadder = FindClosestLadder();
        if (closestLadder != null)
        {
            Vector2 ladderCenter = closestLadder.bounds.center;
            transform.position = new Vector2(ladderCenter.x, transform.position.y);
        }
    }

    public void StopClimbing()
    {
        isExitingClimb = true;// Активируем флаг выхода
        Debug.Log("isClimbing False");
        gameObject.layer = playerLayer;

        // Восстанавливаем коллизии между ClimbingPlayer и Ground
        Physics2D.IgnoreLayerCollision(climbingPlayerLayer, groundLayer, false);

        framingTransposer.m_ScreenY = 0.5682f;
        framingTransposer.m_SoftZoneHeight = 0.5f;
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

    public void OnExitClimbComplete()
    {
        // Вызывается по завершении анимации выхода (через событие анимации)
        isExitingClimb = false;
    }
    public void CamMove()
    {
        // Вызывается по завершении анимации выхода (через событие анимации)
        CameraM = true;
    }
    public void OnExitClimbAnimationComplete()
    {
        // Этот метод вызывается анимацией после её завершения
        isClimbing = false;
        isExitingClimb = false;
        CameraM = false;  // Сбросим флаг движения камеры, если требуется
    }
    // Метод для поиска ближайшей лестницы
    private BoxCollider2D FindClosestLadder()
    {
        BoxCollider2D[] ladders = FindObjectsOfType<BoxCollider2D>();
        BoxCollider2D closestLadder = null;
        float closestDistance = Mathf.Infinity;

        foreach (var ladder in ladders)
        {
            if (ladder.CompareTag("Ladder")) // Убедимся, что это объект с тегом "Ladder"
            {
                float distance = Vector2.Distance(transform.position, ladder.bounds.center);
                if (distance < closestDistance)
                {
                    closestLadder = ladder;
                    closestDistance = distance;
                }
            }
        }

        return closestLadder;
    }
}
