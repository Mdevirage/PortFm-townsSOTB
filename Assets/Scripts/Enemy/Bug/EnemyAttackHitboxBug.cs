using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackHitboxBug : MonoBehaviour
{
    // ������ �� �������� ������ ����� (Sword), ����� ��������� �����
    private BugEnemy parentBug;

    private void Awake()
    {
        // �������� ������ �� ������������ ������
        parentBug = GetComponentInParent<BugEnemy>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ��������, ��� ����� ����� � ���� �����
            parentBug.SetPlayerInAttackZone(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ��������, ��� ����� ����� �� ���� �����
            parentBug.SetPlayerInAttackZone(false);
        }
    }
}
