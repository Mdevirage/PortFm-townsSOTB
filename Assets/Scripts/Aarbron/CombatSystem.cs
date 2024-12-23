using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public Animator animator;               // Ссылка на Animator
    public GameObject hitBoxStand;          // Хитбокс для атаки стоя
    public GameObject hitBoxCrounch;        // Хитбокс для атаки в приседе
    public GameObject hitBoxJump;           // Хитбокс для атаки в прыжке
    public bool isAttacking = false;        // Флаг атаки
    private Rigidbody2D body;
    public bool isAttackingStanding = false;   // Флаг атаки стоя
    public bool isAttackingCrouching = false;  // Флаг атаки в приседе
    public bool isAttackingJumping = false;    // Флаг атаки в прыжке
    public bool isAttackingReverse = false;     // Флаг обратной анимации

    // Особый удар
    public bool canSpecialAttack = false;       // Может ли персонаж выполнять особый удар
    public GameObject energyProjectilePrefab;   // Префаб для снаряда энергии
    public Transform shootPoint;                // Точка, откуда выстрел

    private PlatformerPlayer player;
    private LadderMovement ladderMovement;
    public GameObject chargeEffectObject;
    private HealthManager healthManager;
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GetComponent<PlatformerPlayer>();
        ladderMovement = GetComponent<LadderMovement>();
        healthManager = GetComponent<HealthManager>();
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 0 - индекс слоя
        if (stateInfo.IsName("Aarbron_Turn") || stateInfo.IsName("Aarbron_ClimbUPRevv2") || stateInfo.IsName("Aarbron_descentRevR") ||
            stateInfo.IsName("Aarbron_descentRevL"))
        {
            return;
        }
        if(stateInfo.IsName("TakeDamageStanding")|| stateInfo.IsName("TakeDamageCrouching"))
        {
            return;
        }
        if (!player.isTurning && !healthManager.getdamage)
        {
            HandleCombatInputStanding();
            HandleCombatInputCrouching();

            if (!isAttacking)
            {
                HandleJumpAttackInput();
            }
        }
        if (!isAttacking)
        {
            chargeEffectObject.SetActive(false);
        }
        if (healthManager.getdamage)
        {
            ResetAttack(); // Сбрасываем атаку
            return;
        }
        if (Input.GetKeyUp(KeyCode.X))
        {
            ResetAttack();
        }
        Debug.Log($"isAttackingStanding {isAttackingStanding} & Reverse {isAttackingReverse}");
    }

    private void HandleCombatInputStanding()
    {
        if (Input.GetKey(KeyCode.X) && player.IsGrounded() && !player.isCrouching && !player.isStandingUp && !ladderMovement.isClimbing) // Удар стоя
        {
            shootPoint.transform.localPosition = new Vector2(1.78f, 0.6875f);
            if (!isAttackingStanding)
            {
                body.velocity = Vector2.zero;
                isAttackingStanding = true;
                isAttacking = true;
                animator.SetBool("IsAttackingStanding", true); // Включаем обычную атаку стоя
            }
        }
        else if (isAttackingStanding && Input.GetKeyUp(KeyCode.X))
        {
            if (canSpecialAttack)
            {
                isAttackingReverse = true;
                isAttackingStanding = false;
                isAttacking = false;
                chargeEffectObject.SetActive(false);
                animator.SetBool("IsAttackingStanding", false);
                PerformSpecialAttack(); // Выполняем выстрел энергии
            }
            else
            {
                isAttackingReverse = true;
                isAttackingStanding = false;
                isAttacking = false;
                animator.SetBool("IsAttackingStanding", false);
            }
        }
    }

    private void HandleCombatInputCrouching()
    {
        if (Input.GetKey(KeyCode.X) && player.IsGrounded() && player.isCrouching && !ladderMovement.isClimbing && !player.isTurning) // Удар в приседе
        {
            shootPoint.transform.localPosition = new Vector2(2.0325f, -0.125f);
            if (!isAttackingCrouching)
            {
                isAttackingCrouching = true;
                isAttacking = true;
                animator.SetBool("IsAttackingCrouching", true);
            }
        }
        else if (isAttackingCrouching && Input.GetKeyUp(KeyCode.X))
        {
            if (canSpecialAttack)
            {
                isAttackingCrouching = false;
                isAttacking = false;
                chargeEffectObject.SetActive(false);
                animator.SetBool("IsAttackingCrouching", false);
                PerformSpecialAttack();
                
            }
            else
            {
                isAttackingCrouching = false;
                isAttacking = false;
                animator.SetBool("IsAttackingCrouching", false);
            }
        }
    }

    private void HandleJumpAttackInput()
    {
        // Проверяем, что персонаж не на земле
        bool inAir = !player.IsGrounded();
        if (isAttacking)
            return;

        bool isDescendingOrAtApex = (body.velocity.y > 12f);

        if (inAir && isDescendingOrAtApex && !isAttackingJumping)
        {
            // Если нажата кнопка удара
            if (Input.GetKey(KeyCode.X) && !player.isTurning)
            {
                isAttackingJumping = true;
                isAttacking = true;
                animator.SetBool("IsAttackingJumping", true); // Анимация удара в прыжке
                ActivateHitBoxJump();
            }
        }
    }

    private void PerformSpecialAttack()
    {
        GameObject projectile = Instantiate(energyProjectilePrefab, shootPoint.position, Quaternion.identity);

        // Устанавливаем направление полета снаряда в зависимости от направления персонажа
        Vector3 projectileScale = projectile.transform.localScale;
        projectileScale.x = Mathf.Sign(transform.localScale.x) * Mathf.Abs(projectileScale.x); // Устанавливаем правильный масштаб
        projectile.transform.localScale = projectileScale;
    }
    public void ChargeAnimation()
    {
        if (canSpecialAttack)
        {
            chargeEffectObject.SetActive(true);
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
        if (hitBoxStand != null && !canSpecialAttack)
            hitBoxStand.SetActive(true); // Включаем хитбокс стоя
    }

    public void DeactivateHitBoxStand()
    {
        if (hitBoxStand != null)
            hitBoxStand.SetActive(false); // Отключаем хитбокс стоя
    }

    public void ActivateHitBoxCrounch()
    {
        if (hitBoxCrounch != null && !canSpecialAttack)
            hitBoxCrounch.SetActive(true); // Включаем хитбокс в приседе
    }

    public void DeactivateHitBoxCrounch()
    {
        if (hitBoxCrounch != null)
            hitBoxCrounch.SetActive(false); // Отключаем хитбокс в приседе
    }

    public void AnimationReverseComplete()
    {
        //Debug.Log("isAttackingReverse False");
        isAttackingReverse = false; // Сбрасываем флаг обратной анимации
    }

    public void EndJumpAttack()
    {
        isAttackingJumping = false;
        isAttacking = false;
        animator.SetBool("IsAttackingJumping", false);
        DeactivateHitBoxJump();
    }

    public void ResetAttack()
    {
        if (isAttackingStanding)
        {
            isAttackingStanding = false;
            isAttacking = false;
            isAttackingReverse = false;
            animator.SetBool("IsAttackingStanding", false);
            chargeEffectObject.SetActive(false);
        }
        if (isAttackingCrouching)
        {
            isAttackingCrouching = false;
            isAttacking = false;
            isAttackingReverse = false;
            animator.SetBool("IsAttackingCrouching", false);
            chargeEffectObject.SetActive(false);
        }
        if (isAttackingJumping)
        {
            isAttackingJumping = false;
            isAttacking = false;
            isAttackingReverse = false;
            animator.SetBool("IsAttackingJumping", false);
            DeactivateHitBoxJump();
        }
    }
}