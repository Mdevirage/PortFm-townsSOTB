using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    [Header("Activation Settings")]
    public Transform player;
    public float activationDistance = 10f;

    private bool isActive = false;
    private bool isPlayerInAttackZone = false;

    [Header("Movement Settings")]
    public float speed = 4f;
    public bool moveLeft = true;

    [Header("Attack Settings")]
    public int attackDamage = 1;
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    [Header("Animation Settings")]
    public Animator animator;

    [Header("Death Settings")]
    public GameObject deathEffect;
    public AudioClip deathSound;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Проверяем расстояние до игрока
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        isActive = distanceToPlayer <= activationDistance;

        if (isPlayerInAttackZone)
        {
            Debug.Log("Player in attack zone");
            TryAttack();
            StopMovement();
            return;
        }

        if (isActive)
        {
            Move();
        }
    }

    void Move()
    {
        float direction = moveLeft ? -1 : 1;
        rb.velocity = new Vector2(direction * speed, rb.velocity.y);

        if (animator != null)
        {
            animator.SetBool("IsMoving", true);
        }
    }

    void StopMovement()
    {
        rb.velocity = Vector2.zero;

        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
        }
    }

    private void TryAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    private void Attack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        Debug.Log("Player attacked!");
        if (isPlayerInAttackZone)
        {
            Debug.Log("Player takes damage!");
            player.GetComponent<HealthManager>().TakeDamage(attackDamage);
        }
    }

    public void TakeDamage()
    {
        Die();
    }

    private void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInAttackZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInAttackZone = false;
        }
    }
}
