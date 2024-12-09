using System.Collections;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public int Starthealth = 24;
    public NumberStringDisplay numberStringDisplay;
    public Animator anim;

    private bool isDead = false; // ���� ��������� ������

    void Start()
    {
        anim = GetComponent<Animator>();
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);
    }

    void Update()
    {
        if (isDead) return; // ���� �������� �����, ��������� ����

        // ������ ���������� �������� ��� �����
        if (Input.GetKeyDown(KeyCode.Y) && Starthealth > 0)
        {
            Starthealth -= 1;
            numberStringDisplay.SetDoubleDigitNumber(Starthealth);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            numberStringDisplay.SetKey(true);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            numberStringDisplay.SetKey(false);
        }
    }

    public void TakeDamage()
    {
        if (isDead) return; // ������ ������� ����, ���� �������� ��� �����

        Starthealth -= 1;
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);

        StartCoroutine(numberStringDisplay.BlinkEffect());

        if (Starthealth <= 0)
        {
            Die();
        }
    }

    public void Kill()
    {
        if (isDead) return; // ��� �����

        Starthealth = 0;
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);
        Die();
    }

    void Die()
    {
        isDead = true; // ������������� ���� ������
        anim.SetTrigger("DeathTrigger"); // ��������� �������� ������
        StartCoroutine(HandleDeath());
    }

    // �������� ��� ���������� �������� �� ����� ������������ ��������
    IEnumerator HandleDeath()
    {
        // ������� ��������� �������� ������
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        // �������������� �������� ����� ������
        Debug.Log("Death animation completed. Character is dead.");
    }

    // ����� ��� Animation Event, ���� ����� �������� ������� � ��������
    public void OnDeathAnimationComplete()
    {
        Debug.Log("Animation Event: Death animation finished.");
        gameObject.SetActive(false);
        // ����� �������� �������� ����� ������, ��������, ���������� ������
    }
}
