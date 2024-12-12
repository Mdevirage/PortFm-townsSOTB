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
    private PlatformerPlayer player;
    private LadderMovement ladderMovement;
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
        if (stateInfo.IsName("Aarbron_Turn"))
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
        
    }

    private void HandleCombatInputStanding()
    {
        if (Input.GetKey(KeyCode.X) && player.IsGrounded() && !player.isCrouching && !player.isStandingUp && !ladderMovement.isClimbing) // ���� ����
        {
            if (!isAttackingStanding)
            {
                body.velocity = Vector2.zero;
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
        if (Input.GetKey(KeyCode.X) && player.IsGrounded() && player.isCrouching && !ladderMovement.isClimbing && !player.isTurning) // ���� � �������
        {
            if (!isAttackingCrouching)
            {
                Debug.Log("Standing Attack True");
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
        // ���������, ��� �������� �� �� �����
        bool inAir = !player.IsGrounded();
        if (isAttacking)
            return;
        bool isDescendingOrAtApex = (body.velocity.y > 12f) ;

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
        if (hitBoxStand != null)
            hitBoxStand.SetActive(true); // �������� ������� ����
    }

    public void DeactivateHitBoxStand()
    {
        if (hitBoxStand != null)
            hitBoxStand.SetActive(false); // ��������� ������� ����
    }

    public void ActivateHitBoxCrounch()
    {
        if (hitBoxCrounch != null)
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
