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

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GetComponent<PlatformerPlayer>();
    }

    void Update()
    {
            // ������� ��������� ������� ������ �� ���� ����
        HandleCombatInputStanding();
        HandleCombatInputCrouching();
        // ������ ���� ���� ���� �� �������������, ��������� ��������� �����
        // ��� �������, ��� �������� � ������.
        if (!isAttacking)
        {
            HandleJumpAttackInput();
        }
    }

    private void HandleCombatInputStanding()
    {
        if (Input.GetKey(KeyCode.X) && player.IsGrounded() && !player.isCrouching && !player.isStandingUp) // ���� ����
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
        if (Input.GetKey(KeyCode.X) && player.IsGrounded() && player.isCrouching) // ���� � �������
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
        // ���������, ��� �������� �� �� �����
        bool inAir = !player.IsGrounded();
        if (isAttacking)
            return;
        bool isDescendingOrAtApex = (body.velocity.y > 12f) ;

        if (inAir && isDescendingOrAtApex && !isAttackingJumping)
        {
            // ���� ������ ������ �����
            if (Input.GetKey(KeyCode.X))
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
