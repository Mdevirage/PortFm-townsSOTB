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
    public float jumpHeight = 5f; // ������ ������
    public float jumpSpeed = 2f; // �������� ������
    private bool isJumping = false; // ���� ������
    private bool isFalling = false; // ���� �������
    private float initialY; // ��������� ������
    public float fallSpeed = 2.5f; // �������� �������
    private bool Isdead = false;
    void Start()
    {
        currentHealth = maxHealth; // ������������� ��������� ���������� ������
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); // �������������� Rigidbody2D
        rb.isKinematic = true; // ������������� Rigidbody2D ��� Kinematic
    }

    public void TakeDamage()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("Sphere_GetPunch")) {
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
        firingObject.SetActive(false);
        if (playerCombatSystem != null)
        {
            playerCombatSystem.canSpecialAttack = true; // ��������� ������ �����
        }
        animator.SetTrigger("IsDead");
    }

    void Update()
    {
        // ��������� ������� � ��������
        if (isJumping && !Isdead)
        {
            HandleJump();
        }
        else if (isFalling && !Isdead)
        {
            HandleFall();
        }
    }

    public void StartJump()
    {
        isJumping = true;
        initialY = transform.position.y; // ���������� ��������� ������� �� ��� Y
    }

    void HandleJump()
    {
        // ������ ������� ������ �����
        transform.position += Vector3.up * jumpSpeed * Time.deltaTime;

        // ���� ���������� ������������ ������, �������� �������
        if (transform.position.y >= initialY + jumpHeight)
        {
            isJumping = false;
            isFalling = true;
        }
    }

    void HandleFall()
    {
        // ������ ������� ������ ����
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // ����� ������ ������������ �� ��������� ������, ��������� �������
        if (transform.position.y <= initialY)
        {
            isFalling = false;
            transform.position = new Vector3(transform.position.x, initialY, transform.position.z); // ������������ �������
        }
    }

    public void Death()
    {
        Destroy(gameObject);
    }
}
