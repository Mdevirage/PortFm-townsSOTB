using System.Collections;
using UnityEngine;

/// <summary>
/// Пример скрипта, который обрабатывает столкновения со стеной,
/// проверяет, жмём ли мы кнопку «вперёд» в сторону стены,
/// и при необходимости вызывает логику, увеличивающую счётчик «wallHitCount».
///
/// Примерно объединяет в себе и HealthManager (для счётчика), и CollisionHandler.
/// В реальном проекте можно разделить на 2-3 скрипта.
/// </summary>
public class WallCollisionHandler : MonoBehaviour
{
    /*[Header("WALL SETTINGS")]
    [Tooltip("Tag, который мы считаем 'стеной'")]
    public string wallTag = "Wall";

    [Tooltip("Интервал (секунд) между 'ударами' об стену, если персонаж давит в нее")]
    public float wallHitInterval = 0.5f;
    private float nextWallHitTime = 0f;

    [Header("WALL HIT COUNTER")]
    [Tooltip("Сколько 'псевдо-ударов' нужно, чтобы нанести реальный урон?")]
    public int wallHitThreshold = 5;
    [Tooltip("Текущий счётчик псевдо-ударов об стену.")]
    public int wallHitCount = 0;

    [Header("HEALTH SETTINGS")]
    public int startHealth = 20;            // Здоровье
    public bool isInvincible = false;       // Временная неуязвимость, если нужна
    public bool isDead = false;
    [Tooltip("На сколько секунд персонаж становится неуязвим после удара (пример).")]
    public float invincibilityDuration = 1.5f;

    // Ссылка на Animator (для проигрывания анимаций урона/смерти, если нужно)
    private Animator anim;
    private Rigidbody2D rb;

    void Start()
    {
        // Предположим, что на этом же объекте есть Animator, Rigidbody2D
        // или вы получите ссылки через GetComponentInChildren<Animator>() и т. п.
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Если персонаж уже мёртв — ничего не делаем
        if (isDead) return;

        // Если нужно, чтобы счётчик обнулялся при смерти, можно здесь проверить что угодно.

        // В OnCollisionStay2D мы *определяем* столкновения, но сам инкремент счётчика
        // можно делать там же. Или можно вызывать внутри OnCollisionStay2D -> AttemptWallHit().
        // Однако если вы хотите управлять "раз в N секунд" (cooldown) прямо в OnCollisionStay2D,
        // делайте там.  
        // В данном примере мы сделаем это *прямо* в OnCollisionStay2D
        // (см. метод ниже).
    }

    // -----------------------------------------------------------------------
    //    ЛОГИКА СТОЛКНОВЕНИЙ: OnCollisionStay2D, чтение нормалей
    // -----------------------------------------------------------------------
    void OnCollisionStay2D(Collision2D collision)
    {
        // Проверяем Tag
        if (!collision.gameObject.CompareTag(wallTag)) return;

        // Перебираем точки соприкосновения
        foreach (ContactPoint2D contact in collision.contacts)
        {
            Vector2 normal = contact.normal;
            // Если нормаль вектор примерно горизонтальный (без учёта небольшой погрешности),
            // значит это боковая коллизия, а не пол/потолок.
            if (Mathf.Abs(normal.x) > 0.5f)
            {
                // normal.x > 0 => стена слева (т.к. нормаль смотрит вправо)
                // normal.x < 0 => стена справа (т.к. нормаль смотрит влево)

                bool wallIsOnLeft = (normal.x > 0f);
                bool wallIsOnRight = (normal.x < 0f);

                // Узнаём, жмём ли мы "вперёд"
                if (IsPressingForward(wallIsOnRight, wallIsOnLeft))
                {
                    AttemptWallHit();
                    // Раз нашли подходящий контакт — можно выходить из цикла
                    // (чтобы не считать несколько раз за один Stay)
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Проверка: жмём ли мы кнопку вперёд в сторону стены?
    /// Например, если стена справа, а игрок жмёт "вправо".
    /// </summary>
    private bool IsPressingForward(bool wallOnRight, bool wallOnLeft)
    {
        float inputX = Input.GetAxisRaw("Horizontal");

        // Если стена справа, значит "движение вперёд" = (inputX > 0).
        // Если стена слева, значит "движение вперёд" = (inputX < 0).

        if (wallOnRight && inputX > 0)
            return true;
        if (wallOnLeft && inputX < 0)
            return true;

        return false;
    }

    /// <summary>
    /// Логика: когда мы "давим" в стену, раз в wallHitInterval секунд
    /// инкрементируем счётчик wallHitCount.
    /// </summary>
    private void AttemptWallHit()
    {
        // Проверяем кулдаун
        if (Time.time < nextWallHitTime) return;

        // Обновляем время след. удара
        nextWallHitTime = Time.time + wallHitInterval;

        // Увеличиваем счётчик
        wallHitCount++;
        Debug.Log($"[WallCollision] wallHitCount = {wallHitCount}");

        // Проверяем, не достигнут ли порог для реального урона
        if (wallHitCount >= wallHitThreshold)
        {
            wallHitCount = 0;    // сбрасываем счётчик
            TakeDamageWall();    // наносим реальный урон
        }
    }

    // -----------------------------------------------------------------------
    //    ЛОГИКА УРОНА / ЗДОРОВЬЯ
    // -----------------------------------------------------------------------
    private void TakeDamageWall()
    {
        // Если уже мёртв или неуязвим — не получаем урон.
        if (isDead || isInvincible) return;

        // Уменьшаем здоровье
        startHealth--;
        Debug.Log($"TakeDamage from wall: health = {startHealth}");

        // Если здоровье кончилось — Die()
        if (startHealth <= 0)
        {
            isDead = true;
            anim?.SetTrigger("DeathTrigger");
            // Можно отключить управление, вызвать gameOver и т. п.
            return;
        }

        // Иначе — временная неуязвимость (чтобы за секунду не убило многократным счётом)
        StartCoroutine(InvincibilityRoutine());

        // Запускаем анимацию "получил урон", если хотите
        /*if (anim != null)
        {
            anim.SetTrigger("TakeDamageStanding");
        }
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    // Если хотите обработать момент завершения анимации смерти:
    // вызывайте из Animation Event:
    public void OnDeathAnimationComplete()
    {
        // gameObject.SetActive(false);
        Destroy(gameObject);
    }*/
}
