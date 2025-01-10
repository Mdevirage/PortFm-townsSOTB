using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 4f;

    [Header("Attack Settings")]
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    [Header("Death Settings")]
    public GameObject deathEffect;

    [Header("Detection Settings")]
    public float attackRadius = 1.5f;

    private Animator animator;
    private Rigidbody2D rb;
    public Transform playerTransform;
    public HealthManager health;

    private AudioSource audioSource;
    private bool wasInCameraView = false;

    private bool isAttacking;
    private bool isPlayerInHitbox;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Делаете Rigidbody2D kinematic
        rb.isKinematic = true;
        // Или делаете коллайдер триггером
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        if (playerTransform == null) return;
        if (isPlayerInHitbox)
        {
            if (isAttacking) 
            {
                health?.TakeDamage();
            }
        }
        // Проверяем, в поле зрения ли камера (если нужно)
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        bool isInCameraView = (viewportPos.x >= -0.1f && viewportPos.x <= 1.1f
                            && viewportPos.y >= 0f && viewportPos.y <= 1f
                            && viewportPos.z > 0f);

        if (isInCameraView && !wasInCameraView)
        {
            wasInCameraView = true;
        }

        else if (!isInCameraView && wasInCameraView)
        {
            Destroy(gameObject);
        }

        // Расстояние по X до игрока (если нужно для атаки)
        float distX = playerTransform.position.x - transform.position.x;

        // Если хотим атаковать, когда враг близко
        if ((!isAttacking && distX < 0 && Mathf.Abs(distX) <= attackRadius))
        {
            TryAttack();
        }

        // Если не атакуем, то двигаемся
        if (wasInCameraView)
        {
            Move();
        }
    }

    private void Move()
    {
        if (isAttacking)
        {
            rb.velocity = new Vector2(-speed - 4f, 0f);
        }
        else
        {
            rb.velocity = new Vector2(-speed, 0f);
        }
        animator.SetBool("IsMoving", true);
    }

    private void TryAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            isAttacking = true;
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }

    // Вызывается из анимации
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthManager playerHealth = other.GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                isPlayerInHitbox = true;
                health?.TakeDamage();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthManager playerHealth = other.GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                isPlayerInHitbox = false;
            }
        }
    }
    private void OnAttackComplete()
    {
        isAttacking = false;
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
        Destroy(gameObject);
    }
}
