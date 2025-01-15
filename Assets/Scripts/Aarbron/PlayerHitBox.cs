using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    private bool hasHit = false; // ����, �����������, ��� �� ��� ������ ����
    private void OnEnable()
    {
        // ���������� ���� ��� ��������� �������
        hasHit = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasHit && other.CompareTag("Enemy"))
        {
            hasHit = true; // ������������� ����, ����� ������������� ���������� ���������

            // ��������� ����������� �����
            BugEnemy bugEnemy = other.GetComponent<BugEnemy>();
            Sword sword = other.GetComponent<Sword>();
            Sphere sphere = other.GetComponent<Sphere>();
            Axe axe = other.GetComponent<Axe>();
            BossTree bossTree = other.GetComponent<BossTree>();
            Dragonfly dragonfly = other.GetComponent<Dragonfly>();
            Snake snake = other.GetComponent<Snake>();
            // ��������� �����
            if (bugEnemy != null) bugEnemy.TakeDamage();
            if (sword != null) sword.TakeDamage();
            if (sphere != null) sphere.TakeDamage();
            if (axe != null) axe.TakeDamage();
            if (bossTree != null) bossTree.TakeDamage();
            if (dragonfly != null) dragonfly.TakeDamage();
            if (snake != null) snake.TakeDamage();
            // �����������: ���������� ���������� ����� �����
            // GetComponent<Collider2D>().enabled = false;
        }
    }
}
