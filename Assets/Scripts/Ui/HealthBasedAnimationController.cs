using UnityEngine;

public class HealthBasedAnimationController : MonoBehaviour
{
    public Animator animator;           // ������ �� Animator
    public HealthManager healthManager; // ������ �� HealthManager
    void Update()
    {
        if (healthManager.Starthealth <= 0)
        {
            // ������������� �������� ��� ������
            animator.speed = 0f;
            return;
        }

        if (healthManager.Starthealth <= 5)
        {
            // �������� �������� ��� ������ ������ ��������
            animator.speed = 1.5f; // ��������� (��������� �� �������)
        }
        else
        {
            // ���������� ����������� �������� ��������
            animator.speed = 1f; // ���������� ��������
        }
    }
}
