using UnityEngine;

public class BossDamageZone : MonoBehaviour
{
    [Header("Настройки урона")]
    public float damageInterval = 0.5f;     // Интервал между ударами (в секундах)
    public LayerMask targetLayers;          // Слои, на которые действует урон (например, "Player", "Ally")

    private bool targetInZone = false;      // Флаг, находится ли цель в зоне
    private float damageTimer = 0f;         // Таймер для интервала урона
    public BossTree bossTree;
    private bool hasDead;

    void Update()
    {
        if (targetInZone && !hasDead)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                ApplyDamage();
                damageTimer = 0f;
            }
        }
    }

    void ApplyDamage()
    {
        if (bossTree.isDead)
        {
            hasDead = true;
            return;
        }
        BoxCollider2D box = GetComponent<BoxCollider2D>();

        // Учёт смещения коллайдера
        Vector2 center = (Vector2)transform.position + box.offset;

        // Получаем все объекты в зоне урона, соответствующие targetLayers
        Collider2D[] targets = Physics2D.OverlapBoxAll(center, box.size, 0f, targetLayers);

        foreach (Collider2D target in targets)
        {
            HealthManager health = target.GetComponent<HealthManager>();
            if (health != null && !health.isDead)
            {
                health.TakeDamageBoss(); // Передаём параметр урона
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            targetInZone = true;
            damageTimer = damageInterval; // Немедленный урон при входе
            ApplyDamage(); // Немедленный вызов урона при входе
            Debug.Log(other.gameObject.name + " вошёл в зону урона.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            targetInZone = false;
            damageTimer = 0f;
            Debug.Log(other.gameObject.name + " вышел из зоны урона.");
        }
    }

    // Визуализация зоны урона в редакторе
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            Vector3 center = transform.position + (Vector3)box.offset;
            Gizmos.DrawWireCube(center, box.size);
        }
        else
        {
            // Если нет BoxCollider2D, рисуем стандартный размер
            Gizmos.DrawWireCube(transform.position, Vector3.one);
        }
    }
}
