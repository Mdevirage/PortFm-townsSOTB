using UnityEngine;

public class Sphere : MonoBehaviour
{
    public int maxHealth = 10; // Максимальное количество жизней
    private int currentHealth; // Текущее количество жизней
    public CombatSystem playerCombatSystem; // Ссылка на CombatSystem
    private Animator animator;
    public Animator DemonAnimator;
    public GameObject firingObject;
    private Rigidbody2D rb; // Rigidbody для управления физикой
    private Collider2D sphereCollider;
    [Header("Jump Settings")]
    public AnimationCurve jumpCurve; // Кривая анимации для прыжка
    public float jumpHeight = 5f; // Высота прыжка
    public float jumpDuration = 1f; // Длительность прыжка

    private float jumpTimer; // Таймер прыжка
    private float initialY; // Начальная высота
    private bool isJumping = false; // Флаг прыжка
    private bool Isdead = false;

    void Start()
    {
        currentHealth = maxHealth; // Устанавливаем начальное количество жизней
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); // Инициализируем Rigidbody2D
        sphereCollider = GetComponent<Collider2D>();
        rb.isKinematic = true; // Устанавливаем Rigidbody как Kinematic
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
            playerCombatSystem.canSpecialAttack = true; // Разрешаем особую атаку
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
        jumpTimer = 0f; // Сбрасываем таймер прыжка
        initialY = transform.position.y; // Запоминаем начальную высоту
    }

    void HandleJump()
    {
        jumpTimer += Time.deltaTime;

        // Нормализованное время от 0 до 1
        float normalizedTime = jumpTimer / jumpDuration;
        float curveValue = jumpCurve.Evaluate(normalizedTime); // Получаем значение кривой
        float newY = initialY + curveValue * jumpHeight;

        // Устанавливаем новую позицию по Y
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Когда прыжок завершен
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
