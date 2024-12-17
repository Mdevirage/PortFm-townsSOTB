using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballEnemy : MonoBehaviour
{
    public float speed = 15f; // �������� �������� �������
    private Rigidbody2D rb;
    private Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ActivateMovement();
    }

    public void ActivateMovement()
    {
        if (rb != null)
        {
            float direction = Mathf.Sign(transform.localScale.x); // 1 ��� ������, -1 ��� �����
            rb.velocity = new Vector2(direction * speed, 0); // ������������� ��������
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthManager health = other.GetComponent<HealthManager>();
            if (health != null)
            {
                health.TakeDamage();
            }
        }
    }
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
