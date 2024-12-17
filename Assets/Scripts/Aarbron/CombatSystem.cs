using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public Animator animator;               // ������ �� Animator
    public GameObject hitBoxStand;          // ������� ��� ����� ����
    public GameObject hitBoxCrounch;        // ������� ��� ����� � �������
    public GameObject hitBoxJump;           // ������� ��� ����� � ������
    public bool isAttacking = false;        // ���� �����
    private Rigidbody2D body;
    private bool isAttackingStanding = false;   // ���� ����� ����
    private bool isAttackingCrouching = false;  // ���� ����� � �������
    public bool isAttackingJumping = false;    // ���� ����� � ������
    public bool isAttackingReverse = false;     // ���� �������� ��������

    // ������ ����
    public bool canSpecialAttack = false;       // ����� �� �������� ��������� ������ ����
    public GameObject energyProjectilePrefab;   // ������ ��� ������� �������
    public Transform shootPoint;                // �����, ������ �������

    private PlatformerPlayer player;
    private LadderMovement ladderMovement;
    public GameObject chargeEffectObject;
    public bool isMoving;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GetComponent<PlatformerPlayer>();
        ladderMovement = GetComponent<LadderMovement>();
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 0 - ������ ����
        if (stateInfo.IsName("Aarbron_Turn") || stateInfo.IsName("Aarbron_ClimbUPRevv2") || stateInfo.IsName("Aarbron_descentRevR") ||
            stateInfo.IsName("Aarbron_descentRevL"))
        {
            return;
        }

        if (!player.isTurning)
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
    }

    private void HandleCombatInputStanding()
    {
        if (Input.GetKey(KeyCode.X) && player.IsGrounded() && !player.isCrouching && !player.isStandingUp && !ladderMovement.isClimbing) // ���� ����
        {
            shootPoint.transform.localPosition = new Vector2(1.78f, 0.6875f);
            if (!isAttackingStanding)
            {
                body.velocity = Vector2.zero;
                isAttackingStanding = true;
                isAttacking = true;
                animator.SetBool("IsAttackingStanding", true); // �������� ������� ����� ����
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
                PerformSpecialAttack(); // ��������� ������� �������
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
        if (Input.GetKey(KeyCode.X) && player.IsGrounded() && player.isCrouching && !ladderMovement.isClimbing && !player.isTurning) // ���� � �������
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
        // ���������, ��� �������� �� �� �����
        bool inAir = !player.IsGrounded();
        if (isAttacking)
            return;

        bool isDescendingOrAtApex = (body.velocity.y > 12f);

        if (inAir && isDescendingOrAtApex && !isAttackingJumping)
        {
            // ���� ������ ������ �����
            if (Input.GetKey(KeyCode.X) && !player.isTurning)
            {
                isAttackingJumping = true;
                isAttacking = true;
                animator.SetBool("IsAttackingJumping", true); // �������� ����� � ������
                ActivateHitBoxJump();
            }
        }
    }

    private void PerformSpecialAttack()
    {
        GameObject projectile = Instantiate(energyProjectilePrefab, shootPoint.position, Quaternion.identity);

        // ������������� ����������� ������ ������� � ����������� �� ����������� ���������
        Vector3 projectileScale = projectile.transform.localScale;
        projectileScale.x = Mathf.Sign(transform.localScale.x) * Mathf.Abs(projectileScale.x); // ������������� ���������� �������
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
            hitBoxJump.SetActive(true); // �������� ������� ��� ������
    }

    public void DeactivateHitBoxJump()
    {
        if (hitBoxJump != null)
            hitBoxJump.SetActive(false); // ��������� ������� ��� ������
    }

    public void ActivateHitBoxStand()
    {
        if (hitBoxStand != null && !canSpecialAttack)
            hitBoxStand.SetActive(true); // �������� ������� ����
    }

    public void DeactivateHitBoxStand()
    {
        if (hitBoxStand != null)
            hitBoxStand.SetActive(false); // ��������� ������� ����
    }

    public void ActivateHitBoxCrounch()
    {
        if (hitBoxCrounch != null && !canSpecialAttack)
            hitBoxCrounch.SetActive(true); // �������� ������� � �������
    }

    public void DeactivateHitBoxCrounch()
    {
        if (hitBoxCrounch != null)
            hitBoxCrounch.SetActive(false); // ��������� ������� � �������
    }

    public void AnimationReverseComplete()
    {
        Debug.Log("isAttackingReverse False");
        isAttackingReverse = false; // ���������� ���� �������� ��������
    }

    public void EndJumpAttack()
    {
        isAttackingJumping = false;
        isAttacking = false;
        animator.SetBool("IsAttackingJumping", false);
        DeactivateHitBoxJump();
    }
}