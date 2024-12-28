using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWall : MonoBehaviour
{
    private bool isDamaging = false; // ���� ��� �������������� ������������� ������� ��������
    public float damageCooldown = 0.5f; // ������� � �������� ����� �������
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isDamaging)
        {
            StartCoroutine(DealDamageOverTime(other));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isDamaging)
        {
            isDamaging = false;
            StopAllCoroutines(); // ������������� ��� ��������, ���� ����� ������� �� ����
        }
    }

    private IEnumerator DealDamageOverTime(Collider2D player)
    {
        isDamaging = true;

        while (isDamaging)
        {
            HealthManager playerHealth = player.GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(); // ������� ����
            }

            yield return new WaitForSeconds(damageCooldown); // ��� ����� ��������� ���������� �����
        }
    }
}
