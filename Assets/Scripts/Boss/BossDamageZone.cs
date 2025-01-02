using UnityEngine;

public class BossDamageZone : MonoBehaviour
{
    [Header("��������� �����")]
    public float damageInterval = 0.5f;     // �������� ����� ������� (� ��������)
    public LayerMask targetLayers;          // ����, �� ������� ��������� ���� (��������, "Player", "Ally")

    private bool targetInZone = false;      // ����, ��������� �� ���� � ����
    private float damageTimer = 0f;         // ������ ��� ��������� �����
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

        // ���� �������� ����������
        Vector2 center = (Vector2)transform.position + box.offset;

        // �������� ��� ������� � ���� �����, ��������������� targetLayers
        Collider2D[] targets = Physics2D.OverlapBoxAll(center, box.size, 0f, targetLayers);

        foreach (Collider2D target in targets)
        {
            HealthManager health = target.GetComponent<HealthManager>();
            if (health != null && !health.isDead)
            {
                health.TakeDamageBoss(); // ������� �������� �����
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            targetInZone = true;
            damageTimer = damageInterval; // ����������� ���� ��� �����
            ApplyDamage(); // ����������� ����� ����� ��� �����
            Debug.Log(other.gameObject.name + " ����� � ���� �����.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            targetInZone = false;
            damageTimer = 0f;
            Debug.Log(other.gameObject.name + " ����� �� ���� �����.");
        }
    }

    // ������������ ���� ����� � ���������
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
            // ���� ��� BoxCollider2D, ������ ����������� ������
            Gizmos.DrawWireCube(transform.position, Vector3.one);
        }
    }
}
