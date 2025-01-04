using UnityEngine;
using System.Collections;

public class Dragonfly : MonoBehaviour
{
    [Header("Movement Settings")]
    public float minX = 10f;
    public float maxX = 26f;
    public float upperY = -4.25f;
    public float lowerY = -5.75f;
    public float speed = 5f;
    public float frequency = 1f;
    public float amplitude = 1.25f;

    private int directionX = 1;
    private bool isOnUpperWave = true; // Текущее положение: верхняя или нижняя синусоида
    private bool isSwitchingWave = false; // В процессе переключения между уровнями
    private Animator animator;

    [Header("Death Settings")]
    public GameObject deathEffect;

    void Start()
    {
        transform.position = new Vector3(minX, lowerY, transform.position.z);
        animator = GetComponent<Animator>();
        UpdateAnimation();
    }

    void Update()
    {
        if (isSwitchingWave) return; // Пропускаем обновление, если в процессе переключения

        float newX = transform.position.x + speed * directionX * Time.deltaTime;

        if (newX >= maxX)
        {
            newX = maxX;
            directionX = -1;
            SwitchWave(); // Переход на другую синусоиду
        }
        else if (newX <= minX)
        {
            newX = minX;
            directionX = 1;
            SwitchWave(); // Переход на другую синусоиду
        }

        float waveOffset = Mathf.Sin(newX * frequency) * amplitude;
        float yOffset = isOnUpperWave ? upperY + waveOffset : lowerY + waveOffset;

        transform.position = new Vector3(newX, yOffset, transform.position.z);
    }

    void SwitchWave()
    {
        isSwitchingWave = true;
        if (directionX == 1)
        {
            animator.Play("FlyTurnv3L");
        }
        else
        {
            animator.Play("FlyTurnv3R");
        }
         // Запуск анимации поворота
        StartCoroutine(WaitForTurnAnimation());
    }

    private IEnumerator WaitForTurnAnimation()
    {
        float animationLength = 0.25f;
        float halfDuration = animationLength / 2f; // Делим время на два этапа

        // Начальная позиция
        Vector3 startPosition = transform.position;
        
        // --- Первый этап: движение по диагонали вверх ---
        Vector3 firstEndPosition = startPosition + new Vector3(-0.5f * directionX, 0.5f * directionX, 0); // Смещение вверх

        float elapsedTime = 0f;

        while (elapsedTime < halfDuration)
        {
            // Интерполяция между стартовой и конечной точкой первого этапа
            transform.position = Vector3.Lerp(startPosition, firstEndPosition, elapsedTime / halfDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Убедимся, что объект находится в конечной позиции первого этапа
        transform.position = firstEndPosition;

        // --- Второй этап: движение по диагонали вниз ---
        Vector3 secondEndPosition = firstEndPosition + new Vector3(0.5f * directionX, 0.5f * directionX, 0); // Смещение вниз

        elapsedTime = 0f;

        while (elapsedTime < halfDuration)
        {
            // Интерполяция между конечной точкой первого этапа и конечной точкой второго этапа
            transform.position = Vector3.Lerp(firstEndPosition, secondEndPosition, elapsedTime / halfDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Убедимся, что объект находится в конечной позиции второго этапа
        transform.position = secondEndPosition;

        // Завершаем процесс переключения
        isSwitchingWave = false;
        isOnUpperWave = !isOnUpperWave; // Переключаем синусоиду
        UpdateAnimation(); // Обновляем анимацию движения
    }

    void UpdateAnimation()
    {
        if (animator != null)
        {
            if (directionX == 1) // Движение вправо
            {
                animator.SetInteger("Direction", 1);
                animator.Play("FlyRight");
            }
            else if (directionX == -1) // Движение влево
            {
                animator.SetInteger("Direction", -1);
                animator.Play("FlyLeft");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthManager playerHealth = other.GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
            }
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
        Destroy(gameObject);
    }
}
