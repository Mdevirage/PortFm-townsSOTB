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
    public float jumpHeight = 5f; // Высота прыжка
    public float jumpSpeed = 2f; // Скорость прыжка
    private bool isJumping = false; // Флаг прыжка
    private bool isFalling = false; // Флаг падения
    private float initialY; // Начальная высота
    public float fallSpeed = 2.5f; // Скорость падения
    private bool Isdead = false;
    void Start()
    {
        currentHealth = maxHealth; // Устанавливаем начальное количество жизней
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); // Инициализируем Rigidbody2D
        rb.isKinematic = true; // Устанавливаем Rigidbody2D как Kinematic
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
            playerCombatSystem.canSpecialAttack = true; // Разрешаем особую атаку
        }
        animator.SetTrigger("IsDead");
    }

    void Update()
    {
        // Управляем прыжком и падением
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
        initialY = transform.position.y; // Запоминаем начальную позицию по оси Y
    }

    void HandleJump()
    {
        // Плавно двигаем объект вверх
        transform.position += Vector3.up * jumpSpeed * Time.deltaTime;

        // Если достигнута максимальная высота, начинаем падение
        if (transform.position.y >= initialY + jumpHeight)
        {
            isJumping = false;
            isFalling = true;
        }
    }

    void HandleFall()
    {
        // Плавно двигаем объект вниз
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // Когда объект возвращается на начальную высоту, завершаем падение
        if (transform.position.y <= initialY)
        {
            isFalling = false;
            transform.position = new Vector3(transform.position.x, initialY, transform.position.z); // Корректируем позицию
        }
    }

    public void Death()
    {
        Destroy(gameObject);
    }
}
