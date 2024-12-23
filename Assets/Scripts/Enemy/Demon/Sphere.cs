using UnityEngine;

public class Sphere : MonoBehaviour
{
    public int maxHealth = 10; // ������������ ���������� ������
    private int currentHealth; // ������� ���������� ������
    public CombatSystem playerCombatSystem; // ������ �� CombatSystem
    private Animator animator;
    public Animator DemonAnimator;
    public GameObject firingObject;
    private Rigidbody2D rb; // Rigidbody ��� ���������� �������
    private Collider2D sphereCollider;
    [Header("Jump Settings")]
    public AnimationCurve jumpCurve; // ������ �������� ��� ������
    public float jumpHeight = 5f; // ������ ������
    public float jumpDuration = 1f; // ������������ ������

    private float jumpTimer; // ������ ������
    private float initialY; // ��������� ������
    private bool isJumping = false; // ���� ������
    private bool Isdead = false;

    void Start()
    {
        currentHealth = maxHealth; // ������������� ��������� ���������� ������
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); // �������������� Rigidbody2D
        sphereCollider = GetComponent<Collider2D>();
        rb.isKinematic = true; // ������������� Rigidbody ��� Kinematic
    }

    public void TakeDamage()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("Sphere_GetPunch"))
        {
            currentHealth -= 1;
            animator.SetTrigger("IsDamaging");
            if (currentHealth <= 0)
            {
                DestroyObject();
            }
        }
    }

    private void DestroyObject()
    {
        Isdead = true;
        DemonAnimator.SetTrigger("End");
        sphereCollider.enabled = false;
        firingObject.SetActive(false);
        if (playerCombatSystem != null)
        {
            playerCombatSystem.canSpecialAttack = true; // ��������� ������ �����
        }
        animator.SetTrigger("IsDead");
    }

    void Update()
    {
        if (isJumping && !Isdead)
        {
            HandleJump();
        }
    }

    public void StartJump()
    {
        isJumping = true;
        jumpTimer = 0f; // ���������� ������ ������
        initialY = transform.position.y; // ���������� ��������� ������
    }

    void HandleJump()
    {
        jumpTimer += Time.deltaTime;

        // ��������������� ����� �� 0 �� 1
        float normalizedTime = jumpTimer / jumpDuration;
        float curveValue = jumpCurve.Evaluate(normalizedTime); // �������� �������� ������
        float newY = initialY + curveValue * jumpHeight;

        // ������������� ����� ������� �� Y
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // ����� ������ ��������
        if (normalizedTime >= 1f)
        {
            isJumping = false;
            transform.position = new Vector3(transform.position.x, initialY, transform.position.z);
        }
    }

    public void Death()
    {
        Destroy(gameObject);
    }
}
