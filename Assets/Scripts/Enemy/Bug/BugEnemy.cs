using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 4f;
    public float chaseSpeedMultiplier = 1f;

    [Header("Attack Settings")]
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    [Header("Death Settings")]
    public GameObject deathEffect;        // ���� ���������� ������ (Prefab) ������/������

    [Header("Detection Settings")]
    public float detectionRadius = 5f;
    public float attackRadius = 1.5f;

    private EnemyState currentState = EnemyState.Idle;

    private Animator animator;
    private Rigidbody2D rb;
    public Transform playerTransform;
    public HealthManager health;
    private bool wasInCameraView = false;

    private bool isAttacking;
    private bool isPlayerInAttackRange;
    private bool isPlayerInHitbox;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        if (playerTransform == null) return;
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        bool isInCameraView = (viewportPos.x >= -0.1f && viewportPos.x <= 1.1f
                            && viewportPos.y >= 0f && viewportPos.y <= 1f
                            && viewportPos.z > 0f);

        if (isInCameraView && !wasInCameraView)
        {
            // ������ ������ ��� ���� �����
            wasInCameraView = true;
        }
        else if (!isInCameraView && wasInCameraView)
        {
            Destroy(gameObject);
        }
        if (!isInCameraView)
        {
            return;
        }
        // ������� ���������� �� ������
        //float dist = Vector2.Distance(transform.position, playerTransform.position);
        Vector2 diff = new Vector2(playerTransform.position.x - transform.position.x, 0f);
        float dist = diff.magnitude;  // �� �� �����: ���������� �� X, ��������� Y
        isPlayerInAttackRange = (dist <= attackRadius);

        if (!isAttacking)
        {
            UpdateState(dist);
        }


        // ��������� ������ � ����������� �� �������� ���������
        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdle();
                break;
            case EnemyState.Chase:
                HandleChase();
                break;
            case EnemyState.Attack:
                HandleAttack();
                break;
        }
    }

    private void UpdateState(float distanceToPlayer)
    {
        // ���� ����� � ������� ����� -> Attack
        if (distanceToPlayer <= attackRadius)
        {
            currentState = EnemyState.Attack;
        }
        // ���� ����� ������ � ���� ��������� -> Chase
        else if (distanceToPlayer <= detectionRadius)
        {
            currentState = EnemyState.Chase;
        }
        // ����� -> Idle (��� Patrol)
        else
        {
            currentState = EnemyState.Idle;
        }
    }

    private void HandleIdle()
    {
        StopMovement();
        // ����� �������� �����-�� ������ ������� ��� ������ ������.
    }

    private void HandleChase()
    {
        MoveTowardsPlayer();
        // ��������, ������� ��������
        animator.SetBool("IsMoving", true);
    }

    private void HandleAttack()
    {
        if (!isAttacking)
        {
            // ��������� ��
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                StopMovement();
                isAttacking = true;
                animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
            }
            else
            {
                // ��������, ���� �� �� ����� ��������� (��), ����� ���������� �������������
                // currentState = EnemyState.Chase;
            }
        }
    }

    // ���������� �� ��������
    private void OnAttackHit()
    {
        // ���� ����� ��������� � ���������� �����
        if (isPlayerInHitbox)
        {
            health?.TakeDamage();
        }
    }

    private void OnAttackComplete()
    {
        isAttacking = false;
        // ���� ����� �� ��� � ������� �����, ����� ����� ����
        // ��� ������������� ������� �� Chase, ���� ����� ����� ��������
    }
    public void TakeDamage()
    {
        Die();
    }

    private void Die()
    {
        // ������ ������ (�������, ����� � �. �.)
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // ���������� ������
        Destroy(gameObject);
    }
    private void MoveTowardsPlayer()
    {
        //Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.velocity = new Vector2(-1 * speed, rb.velocity.y);
    }
    public void SetPlayerInAttackZone(bool value)
    {
        isPlayerInHitbox = value;
    }
    private void StopMovement()
    {
        rb.velocity = Vector2.zero;
        animator.SetBool("IsMoving", false);
    }
}
