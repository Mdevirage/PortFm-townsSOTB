using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 8f;
    public float minY = -70.5f;
    public float maxY = -62f;

    [Header("Animation Settings")]
    private Animator animator;

    private bool isMovingDown = false;
    private bool isMovingUp = false;
    private bool isInCameraView = false;
    private bool isTeleporting = false;
    private bool End;

    private void Start()
    {
        animator = GetComponent<Animator>();
        //Debug.Log("Start Position: " + transform.position);
    }

    private void Update()
    {
        CheckCameraView();
        MoveSpikes();
    }

    private void MoveSpikes()
    {
        if (isTeleporting) return; // Прекращаем движение во время телепортации

        if (isInCameraView && !isMovingDown && !isMovingUp)
        {
            isMovingDown = true;
            animator.SetBool("isMovingDown", true);
        }

        if (isMovingDown)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Max(transform.position.y - speed * Time.deltaTime, minY), transform.position.z);

            if (Mathf.Approximately(transform.position.y, minY))
            {
                StartCoroutine(PlayAnimationAndTeleport());
                isMovingDown = false;
            }
        }
        else if (isMovingUp)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Min(transform.position.y + speed * Time.deltaTime, maxY), transform.position.z);

            if (Mathf.Approximately(transform.position.y, maxY))
            {
                isMovingUp = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthManager playerHealth =other.GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                if (!End)
                {
                    playerHealth.TakeDamage();
                }
            }
        }
    }

    private void CheckCameraView()
    {
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(new Vector3(transform.position.x, transform.position.y - 1, transform.position.z)); // Смотрим ниже центра объекта
        isInCameraView = viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1.5f;
    }

    private IEnumerator PlayAnimationAndTeleport()
    {
        isTeleporting = true; // Флаг телепортации

        if (animator != null)
        {
            End = true;
            animator.SetBool("isMovingDown", false);
            animator.SetTrigger("AnimationDrop");
            yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length); // Ждём окончания анимации
        }

        End = false;

        // Телепортируем каплю
        transform.position = new Vector3(transform.position.x, maxY, transform.position.z);

        yield return null; // Ждём один кадр

        isMovingUp = true; // Запускаем движение вверх
        isTeleporting = false; // Заканчиваем телепортацию
    }
}
