using UnityEngine;

public class SpellShot : MonoBehaviour
{
    public float speed = 15f; // Скорость движения снаряда
    private Rigidbody2D rb;
    private Animator animator;
    private bool hasCollided = false; // Флаг для предотвращения повторного вызова анимации
    private AudioSource audioSource;
    public AudioClip[] possibleClips; // Массив аудиоклипов, один из которых выбирается случайно
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void ActivateMovement()
    {
        if (rb != null)
        {
            float direction = Mathf.Sign(transform.localScale.x); // 1 для вправо, -1 для влево
            rb.velocity = new Vector2(direction * speed, 0); // Устанавливаем движение
        }
    }

    void Update()
    {
        CheckOutOfScreen();
    }

    // Проверка выхода объекта за границы экрана
    void CheckOutOfScreen()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);

        // Проверяем, находится ли объект за пределами видимого экрана
        if (screenPoint.x < -0.05 || screenPoint.x > 1.05 || screenPoint.y < 0 || screenPoint.y > 1)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall") || other.CompareTag("Enemy"))
        {
            if (!hasCollided)
            {
                hasCollided = true; // Устанавливаем флаг
                rb.velocity = Vector2.zero; // Останавливаем движение
                if (animator != null)
                {
                    animator.SetTrigger("Explode"); // Запускаем анимацию
                                                    // Если есть хотя бы один клип
                    if (possibleClips != null && possibleClips.Length > 0)
                    {
                        // Выбираем случайный индекс
                        int randomIndex = Random.Range(0, possibleClips.Length);

                        // Подставляем его в AudioSource
                        audioSource.clip = possibleClips[randomIndex];

                        // Проигрываем звук
                        audioSource.Play();
                    }
                }
                else
                {
                    Destroy(gameObject); // На случай отсутствия анимации уничтожаем объект
                }
            }
        }
    }

    // Этот метод вызывается из анимации в конце (Animation Event)
    public void DestroyAfterExplosion()
    {
        Destroy(gameObject); // Уничтожаем объект после окончания анимации
    }
}
