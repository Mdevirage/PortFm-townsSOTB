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

    private bool jumpTriggered = false;     // ����, �����������, ��� ������ ��������
    private float jumpAttackWindow = 0.1f;  // ��������� ���� ��� ����� ����� ������
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
        if (Input.GetKeyDown(KeyCode.X) && player.IsGrounded() && !player.isCrouching && !player.isStandingUp) // ���� ����
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
        if (Input.GetKeyDown(KeyCode.X) && player.IsGrounded() && player.isCrouching) // ���� � �������
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
        // ���� �������� � �������, ���������� ���� ��� �����
        if (player.IsGrounded() && Input.GetKey(KeyCode.Z) && !player.isCrouching && !Input.GetKey(KeyCode.X))
        {
            Debug.Log("Jump triggered");
            jumpTriggered = true;
            jumpAttackTimer = jumpAttackWindow; // ������������� ���� �������
        }
        // ���� ������ ������ ����� � ������� ����
        if (jumpTriggered && Input.GetKeyDown(KeyCode.X) && jumpAttackTimer > 0)
        {
            isAttackingJumping = true;
            Debug.Log("Jump attack triggered");
            isAttacking = true;
            animator.SetBool("IsAttackingJumping", true); // �������� ����� � ������
            ActivateHitBoxJump();
            jumpTriggered = false; // ���������� ���� ������
        }

        // ��������� ������ ���� �����
        if (jumpAttackTimer > 0)
        {
            jumpAttackTimer -= Time.deltaTime;
        }
        else
        {
            jumpTriggered = false; // ���������� ����, ���� ����� �����
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
