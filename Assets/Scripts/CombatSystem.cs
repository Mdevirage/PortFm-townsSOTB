using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public Animator animator;               // Ссылка на Animator
    public GameObject hitBoxStand;          // Хитбокс для атаки стоя
    public GameObject hitBoxCrounch;        // Хитбокс для атаки в приседе
    public GameObject hitBoxJump;           // Хитбокс для атаки в прыжке
    public bool isAttacking = false;        // Флаг атаки
    private Rigidbody2D body;
    private bool isAttackingStanding = false;   // Флаг атаки стоя
    private bool isAttackingCrouching = false;  // Флаг атаки в приседе
    public bool isAttackingJumping = false;    // Флаг атаки в прыжке
    public bool isAttackingReverse = false;     // Флаг обратной анимации
    private PlatformerPlayer player;

    private bool jumpTriggered = false;     // Флаг, указывающий, что прыжок выполнен
    private float jumpAttackWindow = 0.1f;  // Временное окно для удара после прыжка
    private float jumpAttackTimer = 0.125f;
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GetComponent<PlatformerPlayer>();
    }

    void Update()
    {
        HandleCombatInputStanding();
        HandleCombatInputCrouching();
        HandleJumpAttackInput();
    }

    private void HandleCombatInputStanding()
    {
        if (Input.GetKeyDown(KeyCode.X) && player.IsGrounded() && !player.isCrouching && !player.isStandingUp) // Удар стоя
        {
            if (!isAttackingStanding)
            {
                isAttackingStanding = true;
                isAttacking = true;
                animator.SetBool("IsAttackingStanding", true);
            }
        }
        else if (isAttackingStanding && Input.GetKeyUp(KeyCode.X))
        {
            isAttackingReverse = true;
            isAttackingStanding = false;
            isAttacking = false;
            animator.SetBool("IsAttackingStanding", false);
        }
    }

    private void HandleCombatInputCrouching()
    {
        if (Input.GetKeyDown(KeyCode.X) && player.IsGrounded() && player.isCrouching) // Удар в приседе
        {
            Debug.Log("Crouching Attack");
            if (!isAttackingCrouching)
            {
                isAttackingCrouching = true;
                isAttacking = true;
                animator.SetBool("IsAttackingCrouching", true);
            }
        }
        else if (isAttackingCrouching && Input.GetKeyUp(KeyCode.X))
        {
            isAttackingCrouching = false;
            isAttacking = false;
            animator.SetBool("IsAttackingCrouching", false);
        }
    }

    private void HandleJumpAttackInput()
    {
        // Если персонаж в воздухе, активируем окно для удара
        if (player.IsGrounded() && Input.GetKey(KeyCode.Z) && !player.isCrouching && !Input.GetKey(KeyCode.X))
        {
            Debug.Log("Jump triggered");
            jumpTriggered = true;
            jumpAttackTimer = jumpAttackWindow; // Устанавливаем окно времени
        }
        // Если нажата кнопка атаки в течение окна
        if (jumpTriggered && Input.GetKeyDown(KeyCode.X) && jumpAttackTimer > 0)
        {
            isAttackingJumping = true;
            Debug.Log("Jump attack triggered");
            isAttacking = true;
            animator.SetBool("IsAttackingJumping", true); // Анимация удара в прыжке
            ActivateHitBoxJump();
            jumpTriggered = false; // Сбрасываем флаг прыжка
        }

        // Обновляем таймер окна удара
        if (jumpAttackTimer > 0)
        {
            jumpAttackTimer -= Time.deltaTime;
        }
        else
        {
            jumpTriggered = false; // Сбрасываем флаг, если время вышло
        }
    }
    public void ActivateHitBoxJump()
    {
        if (hitBoxJump != null)
            hitBoxJump.SetActive(true); // Включаем хитбокс для прыжка
    }

    public void DeactivateHitBoxJump()
    {
        if (hitBoxJump != null)
            hitBoxJump.SetActive(false); // Отключаем хитбокс для прыжка
    }

    public void ActivateHitBoxStand()
    {
        if (hitBoxStand != null)
            hitBoxStand.SetActive(true); // Включаем хитбокс стоя
    }

    public void DeactivateHitBoxStand()
    {
        if (hitBoxStand != null)
            hitBoxStand.SetActive(false); // Отключаем хитбокс стоя
    }

    public void ActivateHitBoxCrounch()
    {
        if (hitBoxCrounch != null)
            hitBoxCrounch.SetActive(true); // Включаем хитбокс в приседе
    }

    public void DeactivateHitBoxCrounch()
    {
        if (hitBoxCrounch != null)
            hitBoxCrounch.SetActive(false); // Отключаем хитбокс в приседе
    }

    public void AnimationReverseComplete()
    {
        isAttackingReverse = false; // Сбрасываем флаг обратной анимации
    }
    public void EndJumpAttack()
    {
        isAttackingJumping = false;
        isAttacking = false;
        animator.SetBool("IsAttackingJumping", false);
        DeactivateHitBoxJump();
    }

}
