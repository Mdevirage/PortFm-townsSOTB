using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 6f;
    public float minY = -35f;
    public float maxY = -29f; // Added maxY

    private bool isMovingDown = false;
    private bool isMovingUp = false; // Added isMovingUp
    private bool isInCameraView = false;
    private void Update()
    {
        CheckCameraView();
        MoveSpikes();
    }

    private void MoveSpikes()
    {
        if (isInCameraView && !isMovingDown && !isMovingUp) // Only start moving down if not already moving
        {
            isMovingDown = true;
        }

        if (isMovingDown)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector3(transform.position.x, minY), speed * Time.deltaTime);
            if (transform.position.y <= minY + 0.01f)
            {
                isMovingDown = false;
                isMovingUp = true; // Start moving up
            }
        }
        else if (isMovingUp)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, maxY), speed * Time.deltaTime);
            if (transform.position.y >= maxY - 0.01f)
            {
                isMovingUp = false;
            }
        }
        else if (!isInCameraView && isMovingDown)
        {
            isMovingDown = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            KillPlayer(other);
        }
    }

    private void KillPlayer(Collider2D playerCollider)
    {
        HealthManager playerHealth = playerCollider.GetComponent<HealthManager>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage();
        }
    }

    private void CheckCameraView()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            // Получаем нижнюю точку объекта
            Vector3 bottomPoint = new Vector3(
                collider.bounds.center.x,
                collider.bounds.min.y, // Нижняя граница коллайдера
                collider.bounds.center.z
            );

            // Проверяем видимость этой точки
            Vector3 viewportPoint = Camera.main.WorldToViewportPoint(bottomPoint);

            isInCameraView = viewportPoint.x >= 0 && viewportPoint.x <= 1 &&
                             viewportPoint.y >= 0 && viewportPoint.y <= 1 &&
                             viewportPoint.z > 0;
        }
    }
}