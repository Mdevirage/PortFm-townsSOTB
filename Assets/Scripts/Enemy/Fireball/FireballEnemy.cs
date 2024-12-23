using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballEnemy : MonoBehaviour
{
    public float speed = 15f; // Скорость движения снаряда
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
            float direction = Mathf.Sign(transform.localScale.x); // 1 для вправо, -1 для влево
            rb.velocity = new Vector2(direction * speed, 0); // Устанавливаем движение
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

    void Update()
    {
        CheckOutOfScreen();
    }

    void CheckOutOfScreen()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        if (screenPoint.x < 0 || screenPoint.x > 1.1f || screenPoint.y < 0 || screenPoint.y > 1)
        {
            Destroy(gameObject); // Удаляем объект, если он за пределами экрана
        }
    }
}
