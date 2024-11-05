using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlatformerPlayer : MonoBehaviour
{
    public float speed = 4.5f;
    public float jumpForce = 12.0f;
    public LayerMask groundLayer;

    private BoxCollider2D box;
    private Rigidbody2D body;
    private Animator anim;
    private bool isTurning;
    private bool wasGrounded;
    private bool isCrouching = false;
    private bool isStandingUp = false; // Переменная для блокировки действий во время StandUp
    private Vector2 groundedPosition;
    private float previousDirection;
    private LadderMovement Ladder;  // Ссылка на LadderMovement

    private LandingSound landingSound;

    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    void Start()
    {
        box = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        Ladder = GetComponent<LadderMovement>();  // Получаем компонент LadderMovement
        previousDirection = transform.localScale.x;
        isTurning = false;
        wasGrounded = true;
        landingSound = GetComponent<LandingSound>();

        originalColliderSize = box.size;
        originalColliderOffset = box.offset;
    }

    void Update()
    {

        // Если персонаж лазает по лестнице, блокируем управление движением и прыжки
        if (Ladder.isClimbing)
        {
            body.velocity = Vector2.zero; // Останавливаем движение по земле
            anim.SetFloat("Speed", 0);    // Устанавливаем скорость анимации в 0
            return; // Прерываем выполнение Update, если персонаж на лестнице
        }

        // Если персонаж выполняет StandUp, блокируем все действия
        if (isStandingUp)
        {
            body.velocity = new Vector2(0, body.velocity.y);
            anim.SetFloat("Speed", 0);
            return;
        }
        // Оставшийся код для обычного передвижения персонажа
        Vector2 boxCenter = new Vector2(box.bounds.center.x, box.bounds.min.y);
        Vector2 boxSize = new Vector2(box.bounds.size.x, 0.1f);
        RaycastHit2D hit = Physics2D.BoxCast(boxCenter, boxSize, 0, Vector2.down, 0.1f, groundLayer);
        bool grounded = hit.collider != null;
        bool wasGroundedPreviously = wasGrounded;
        anim.SetBool("IsGrounded", grounded);

        if (!wasGroundedPreviously && grounded)
        {
            landingSound.PlayLandingSound();
        }

        wasGrounded = grounded;

        // Приседания
        if (grounded && (Input.GetKey(KeyCode.DownArrow) && !Ladder.isClimbing && !Ladder.isBottomDetectorActive))
        {
            body.velocity = new Vector2(0, body.velocity.y);
            Crouch();
            return;
        }
        else if (isCrouching && grounded && !Ladder.isClimbing)
        {
            StandUp();
            return;
        }

        float deltaX = Input.GetAxis("Horizontal") * speed;
        Vector2 movement = new Vector2(deltaX, body.velocity.y);

        if (grounded)
        {
            groundedPosition = transform.position;
        }
        
        if (!grounded && transform.position.y < groundedPosition.y)
        {
            movement.x = 0;
            anim.SetTrigger("IsJumping");
            body.velocity = new Vector2(0, body.velocity.y);
        }

        // Повороты и движения
        if (!isTurning && grounded)
        {
            bool isTurningNow = (deltaX > 0 && previousDirection < 0) || (deltaX < 0 && previousDirection > 0);

            if (isTurningNow)
            {
                isTurning = true;
                anim.SetTrigger("Turn");
                previousDirection = Mathf.Sign(deltaX);
            }
        }

        if (!isTurning && grounded)
        {
            anim.SetFloat("Speed", Mathf.Abs(deltaX));
            body.velocity = movement;
        }

        // Прыжок
        if (grounded && Input.GetKeyDown(KeyCode.Space) && !isCrouching & !Ladder.isClimbing)
        {
            anim.SetTrigger("IsJumping");
            body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // Движение на движущейся платформе
        MovingPlatform platform = null;
        if (grounded && hit.collider != null)
        {
            platform = hit.collider.GetComponent<MovingPlatform>();
        }
        if (platform != null)
        {
            transform.parent = platform.transform;
        }
        else
        {
            transform.parent = null;
        }

        // Обработка поворота на платформе
        if (grounded && !Mathf.Approximately(deltaX, 0))
        {
            transform.localScale = new Vector3(Mathf.Sign(deltaX), 1, 1);
        }
    }

    void Crouch()
    {
        if (isCrouching) return;

        isCrouching = true;
        anim.SetBool("IsCrouching", true);

        float crouchHeight = originalColliderSize.y / 2;
        box.size = new Vector2(originalColliderSize.x, crouchHeight);
        box.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - (originalColliderSize.y - crouchHeight) / 2);
    }

    void StandUp()
    {
        if (!isCrouching) return;
        isStandingUp = true;
        isCrouching = false;
        anim.SetTrigger("StandUpTrigger");
    }

    public void OnStandUpStart()
    {
        isStandingUp = true;
    }

    public void OnStandUpEnd()
    {
        isStandingUp = false;

        box.size = originalColliderSize;
        box.offset = originalColliderOffset;
        anim.SetBool("IsCrouching", false);
    }

    public void EndTurn()
    {
        isTurning = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (box != null)
        {
            Vector2 boxCenter = new Vector2(box.bounds.center.x, box.bounds.min.y);
            Vector2 boxSize = new Vector2(box.bounds.size.x, 0.1f);
            Gizmos.DrawWireCube(boxCenter + Vector2.down * 0.1f / 2, boxSize);
        }
    }
}
