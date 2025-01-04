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

        if (healthManager.Starthealth <= 20)
        {
            // �������� �������� ��� ������ ������ ��������
            animator.speed = 1.125f; // ��������� (��������� �� �������)
        }
        else if (healthManager.Starthealth <= 15)
        {
            // ���������� ����������� �������� ��������
            animator.speed = 1.25f; // ���������� ��������
        }
        else if (healthManager.Starthealth <= 10)
        {
            animator.speed = 1.5f;
        }
        else if(healthManager.Starthealth <= 5)
        {
            animator.speed = 1.75f;
        }
    }
}
