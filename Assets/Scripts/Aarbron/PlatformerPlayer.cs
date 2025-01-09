using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlatformerPlayer : MonoBehaviour
{
    public float speed = 4.5f;
    public float jumpForce = 12.0f;
    public LayerMask groundLayer;
    private BoxCollider2D box;
    public Rigidbody2D body;
    private Animator anim;
    public bool isTurning;
    private bool wasGrounded;
    public bool isCrouching = false;
    public bool isStandingUp = false; // Переменная для блокировки действий во время StandUp
    private Vector2 groundedPosition;
    public float previousDirection;
    private LadderMovement Ladder;  // Ссылка на LadderMovement
    public LandingSound landingSound;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    public bool isFalling;
    private CombatSystem combatSystem;
    public bool isMovementLocked = false;
    private HealthManager healthManager;
    public bool isJumping = false;
    public bool isMoveTrigger = false;
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
        combatSystem = GetComponent<CombatSystem>();
        healthManager = GetComponent<HealthManager>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && Mathf.Abs(Input.GetAxis("Horizontal")) >= 1)
        {
            isMoveTrigger = false;
        }
        if (healthManager.isDead) {  return; }
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("TakeDamageStanding")) 
        {
            body.velocity = new Vector2(0, body.velocity.y);
            anim.SetFloat("Speed", 0);
            return; 
        }
        if ((combatSystem.isAttacking || combatSystem.isAttackingReverse))
        {
            if (!combatSystem.isAttackingJumping)
            {
                // Блокируем движение, если персонаж атакует
                body.velocity = new Vector2(0, body.velocity.y);
                return;
            }
            else
            {
                return;
            }
        }
        if (isMovementLocked)
        {
            body.velocity = new Vector2(0, body.velocity.y);
            anim.SetFloat("Speed", 0);
            return;
        }
        

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

        if (!Ladder.isClimbing && !Ladder.isTopDetectorActive)
        {
            HandleStandUpInput();
        }

        // Оставшийся код для обычного передвижения персонажа
        Vector2 boxCenter = new Vector2(box.bounds.center.x, box.bounds.min.y);
        Vector2 boxSize = new Vector2(box.bounds.size.x, 0.1f);
        RaycastHit2D hit = Physics2D.BoxCast(boxCenter, boxSize, 0, Vector2.down, 0.05f, groundLayer);
        bool grounded = hit.collider != null && hit.distance <= 0.1f;
        bool wasGroundedPreviously = wasGrounded;
        anim.SetBool("IsGrounded", grounded);

        if (!wasGroundedPreviously && grounded)
        {
            if (isFalling)
            {
                landingSound.PlayLandingSound();
                isFalling = false;
                isJumping = false;
                body.velocity = new Vector2(0, body.velocity.y);
                EndJump();
            }
            else
            {
                if (isJumping) 
                {
                    landingSound.PlayLandingSound();
                }
                body.velocity = new Vector2(0, body.velocity.y);
                isJumping = false;
                EndJump();
            }
        }
        if (grounded != wasGrounded)
        {
            //Debug.Log($"Grounded state changed: {grounded}");
            wasGrounded = grounded;
        }
        //wasGrounded = grounded;

        // Приседания
        if (grounded && (Input.GetButton("Down") && !Ladder.isClimbing && !Ladder.isBottomDetectorActive) && !Input.GetButton("Up"))
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
        Vector2 movement;
        if (!isMoveTrigger) 
        {
            movement = new Vector2(deltaX, body.velocity.y);
        }
        else
        {
            movement = new Vector2(0, body.velocity.y);
        }
       
        
        if (grounded)
        {
            groundedPosition = transform.position;
        }
        
        if (!grounded && transform.position.y < groundedPosition.y)
        {
            // Останавливаем горизонтальное движение
            Debug.Log("IsFalling");
            movement.x = 0;
            //Debug.Log("Falling");
            // Триггер анимации прыжка
            anim.SetTrigger("IsJumping");
            isFalling = true;
            // Устанавливаем фиксированную скорость падения
            float fixedFallSpeed = -15f; // Фиксированная скорость падения
            body.velocity = new Vector2(0, fixedFallSpeed);
            EndJump();
        }

        // Повороты и движения
        if (!isTurning && grounded)
        {
            bool isTurningNow = (deltaX > 0 && previousDirection < 0) || (deltaX < 0 && previousDirection > 0);

            if (isTurningNow)
            {
                isTurning = true;
                anim.SetTrigger("Turn");
            }
        }

        if (!isTurning && grounded)
        {
            anim.SetFloat("Speed", Mathf.Abs(deltaX));
            body.velocity = new Vector2(Mathf.Clamp(movement.x, -speed, speed), body.velocity.y);
        }
        if (!Input.GetButton("Attack")){
            // Прыжок
            if (grounded && Input.GetButton("Jump") && !isCrouching && !Ladder.isClimbing && !isTurning)
            {
                isJumping = true;
                anim.SetTrigger("IsJumping");
                body.velocity = new Vector2(body.velocity.x, 0);
                body.velocity = new Vector2(body.velocity.x, jumpForce);
                StartJump();
            }
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

    void HandleStandUpInput()
    {
        if (isCrouching && Input.GetButton("Up"))
        {
            StandUp();
        }
    }
    void Crouch()
    {
        if (isCrouching || isStandingUp || Input.GetButton("Up")) return;

        isCrouching = true;
        anim.SetBool("IsCrouching", true);

        float crouchHeight = originalColliderSize.y / 2;
        box.size = new Vector2(originalColliderSize.x, crouchHeight);
        box.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - (originalColliderSize.y - crouchHeight) / 2);
    }
    void StandUp()
    {
        if (!isCrouching || isStandingUp) return;

        isCrouching = false;
        isStandingUp = true;
        anim.SetTrigger("StandUpTrigger");
        isMovementLocked = true; // Блокируем движение
        StartCoroutine(ResetStandingStateAfterDelay());
    }

    IEnumerator ResetStandingStateAfterDelay()
    {
        yield return new WaitForSeconds(0.2f); // Настройте длительность при необходимости
        box.size = originalColliderSize;
        box.offset = originalColliderOffset;
        isStandingUp = false;
        anim.SetBool("IsCrouching", false);
    }
    public bool IsGrounded()
    {
        Vector2 boxCenter = new Vector2(box.bounds.center.x, box.bounds.min.y);
        Vector2 boxSize = new Vector2(box.bounds.size.x, 0.1f);
        RaycastHit2D hit = Physics2D.BoxCast(boxCenter, boxSize, 0, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;
    }

    public void EndTurn()
    {
        Debug.Log("IsTurning False");
        isTurning = false;
        previousDirection = transform.localScale.x;
    }

    private void StartJump()
    {
        gameObject.layer = LayerMask.NameToLayer("JumpingPlayer");
    }
    public void EndJump()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    public void UnlockMovement()
    {
        isMovementLocked = false; // Разблокируем движение
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (box != null)
        {
            Vector2 boxCenter = new Vector2(box.bounds.center.x, box.bounds.min.y);
            Vector2 boxSize = new Vector2(box.bounds.size.x, 0.1f);
            Gizmos.DrawWireCube(boxCenter + (Vector2.down * 0.1f / 2), boxSize);
        }
    }
}
