using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackHitBoxAxe : MonoBehaviour
{
    private Axe parentAxe;
    // Start is called before the first frame update
    private void Awake()
    {
        // �������� ������ �� ������������ ������
        parentAxe = GetComponentInParent<Axe>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ��������, ��� ����� ����� � ���� �����
            parentAxe.SetPlayerInAttackZone(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ��������, ��� ����� ����� �� ���� �����
            parentAxe.SetPlayerInAttackZone(false);
        }
    }
}
