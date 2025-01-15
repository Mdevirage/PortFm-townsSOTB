using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ExplosionData
{
    public Transform explosionPoint;      // �����, ��� ����� ����������� �����
    public GameObject explosionPrefab;    // ������ ������, ������� ����� �������������
    public float delayAfterPrevious = 0f; // �������� ����� ���� �������
}

public class BossTree : MonoBehaviour
{
    public int Health = 1;
    public bool isDead = false;

    [Header("������� ������")]
    public List<ExplosionData> explosions; // ������ ������ � �������

    [Header("���������� ��� �������� ������")]
    public SpriteRenderer spriteRenderer1;
    public SpriteRenderer spriteRenderer2;
    public CombatSystem combatSystemplayer;
    public void TakeDamage()
    {
        if (isDead)
            return;

        Health -= 1;
        Debug.Log("���� ������� ����. ������� ��������: " + Health);

        if (!isDead && Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        isDead = true;
        // ��������� �������� ��� ���������������� �������
        StartCoroutine(TriggerDeathEffectsCoroutine());
        combatSystemplayer.canSpecialAttack = false;
        // ��������� ������� �����
        spriteRenderer1.enabled = false;
        spriteRenderer2.enabled = false;
        
    }

    IEnumerator TriggerDeathEffectsCoroutine()
    {
        foreach (ExplosionData explosion in explosions)
        {
            if (explosion.explosionPrefab != null && explosion.explosionPoint != null)
            {
                // ��� �������� ����� �������, ���� ��� ������
                if (explosion.delayAfterPrevious > 0f)
                {
                    yield return new WaitForSeconds(explosion.delayAfterPrevious);
                }

                // ������������ ������ ������ � ������ �����
                Instantiate(explosion.explosionPrefab, explosion.explosionPoint.position, Quaternion.identity);
                Debug.Log($"����� ������ � �����: {explosion.explosionPoint.position}");
            }
            else
            {
                Debug.LogWarning("BossTree: ������ ������ ��� ����� �� ���������.");
            }
        }

        // ����� ���� ������� ���������� ������ �����
        Destroy(transform.parent.gameObject);
        Debug.Log("���� ��������� ����� ���� �������.");
    }
}
