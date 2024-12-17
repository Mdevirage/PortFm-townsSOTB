using UnityEngine;

public class SpellShot : MonoBehaviour
{
    public float speed = 15f; // Скорость движения снаряда
    private Rigidbody2D rb;
    private Animator animator;
    private bool hasCollided = false; // Флаг для предотвращения повторного вызова анимации
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void ActivateMovement()
    {
        if (rb != null)
        {
            float direction = Mathf.Sign(transform.localScale.x); // 1 для вправо, -1 для влево
            rb.velocity = new Vector2(direction * speed, 0); // Устанавливаем движение
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

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
