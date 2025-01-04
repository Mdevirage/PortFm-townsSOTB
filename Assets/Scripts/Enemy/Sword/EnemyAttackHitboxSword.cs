using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackHitboxSword : MonoBehaviour
{
    // ������ �� �������� ������ ����� (Sword), ����� ��������� �����
    private Sword parentSword;

    private void Awake()
    {
        // �������� ������ �� ������������ ������
        parentSword = GetComponentInParent<Sword>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ��������, ��� ����� ����� � ���� �����
            parentSword.SetPlayerInAttackZone(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ��������, ��� ����� ����� �� ���� �����
            parentSword.SetPlayerInAttackZone(false);
        }
    }
}
