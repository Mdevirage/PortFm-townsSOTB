using UnityEngine;
using System.Collections;

public class Dragonfly : MonoBehaviour
{
    [Header("Movement Settings")]
    public float minX = 10f;
    public float maxX = 26f;
    public float upperY = -4.25f;
    public float lowerY = -5.75f;
    public float speed = 5f;
    public float frequency = 1f;
    public float amplitude = 1.25f;

    private int directionX = 1;
    private bool isOnUpperWave = true; // ������� ���������: ������� ��� ������ ���������
    private bool isSwitchingWave = false; // � �������� ������������ ����� ��������
    private Animator animator;

    [Header("Death Settings")]
    public GameObject deathEffect;

    void Start()
    {
        transform.position = new Vector3(minX, lowerY, transform.position.z);
        animator = GetComponent<Animator>();
        UpdateAnimation();
    }

    void Update()
    {
        if (isSwitchingWave) return; // ���������� ����������, ���� � �������� ������������

        float newX = transform.position.x + speed * directionX * Time.deltaTime;

        if (newX >= maxX)
        {
            newX = maxX;
            directionX = -1;
            SwitchWave(); // ������� �� ������ ���������
        }
        else if (newX <= minX)
        {
            newX = minX;
            directionX = 1;
            SwitchWave(); // ������� �� ������ ���������
        }

        float waveOffset = Mathf.Sin(newX * frequency) * amplitude;
        float yOffset = isOnUpperWave ? upperY + waveOffset : lowerY + waveOffset;

        transform.position = new Vector3(newX, yOffset, transform.position.z);
    }

    void SwitchWave()
    {
        isSwitchingWave = true;
        if (directionX == 1)
        {
            animator.Play("FlyTurnv3L");
        }
        else
        {
            animator.Play("FlyTurnv3R");
        }
         // ������ �������� ��������
        StartCoroutine(WaitForTurnAnimation());
    }

    private IEnumerator WaitForTurnAnimation()
    {
        float animationLength = 0.25f;
        float halfDuration = animationLength / 2f; // ����� ����� �� ��� �����

        // ��������� �������
        Vector3 startPosition = transform.position;
        
        // --- ������ ����: �������� �� ��������� ����� ---
        Vector3 firstEndPosition = startPosition + new Vector3(-0.5f * directionX, 0.5f * directionX, 0); // �������� �����

        float elapsedTime = 0f;

        while (elapsedTime < halfDuration)
        {
            // ������������ ����� ��������� � �������� ������ ������� �����
            transform.position = Vector3.Lerp(startPosition, firstEndPosition, elapsedTime / halfDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ��������, ��� ������ ��������� � �������� ������� ������� �����
        transform.position = firstEndPosition;

        // --- ������ ����: �������� �� ��������� ���� ---
        Vector3 secondEndPosition = firstEndPosition + new Vector3(0.5f * directionX, 0.5f * directionX, 0); // �������� ����

        elapsedTime = 0f;

        while (elapsedTime < halfDuration)
        {
            // ������������ ����� �������� ������ ������� ����� � �������� ������ ������� �����
            transform.position = Vector3.Lerp(firstEndPosition, secondEndPosition, elapsedTime / halfDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ��������, ��� ������ ��������� � �������� ������� ������� �����
        transform.position = secondEndPosition;

        // ��������� ������� ������������
        isSwitchingWave = false;
        isOnUpperWave = !isOnUpperWave; // ����������� ���������
        UpdateAnimation(); // ��������� �������� ��������
    }

    void UpdateAnimation()
    {
        if (animator != null)
        {
            if (directionX == 1) // �������� ������
            {
                animator.SetInteger("Direction", 1);
                animator.Play("FlyRight");
            }
            else if (directionX == -1) // �������� �����
            {
                animator.SetInteger("Direction", -1);
                animator.Play("FlyLeft");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthManager playerHealth = other.GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
            }
        }
    }

    public void TakeDamage()
    {
        Die();
    }

    private void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
