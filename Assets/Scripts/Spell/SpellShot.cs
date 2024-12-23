using UnityEngine;

public class SpellShot : MonoBehaviour
{
    public float speed = 15f; // �������� �������� �������
    private Rigidbody2D rb;
    private Animator animator;
    private bool hasCollided = false; // ���� ��� �������������� ���������� ������ ��������

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void ActivateMovement()
    {
        if (rb != null)
        {
            float direction = Mathf.Sign(transform.localScale.x); // 1 ��� ������, -1 ��� �����
            rb.velocity = new Vector2(direction * speed, 0); // ������������� ��������
        }
    }

    void Update()
    {
        CheckOutOfScreen();
    }

    // �������� ������ ������� �� ������� ������
    void CheckOutOfScreen()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);

        // ���������, ��������� �� ������ �� ��������� �������� ������
        if (screenPoint.x < -0.05 || screenPoint.x > 1.05 || screenPoint.y < 0 || screenPoint.y > 1)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall") || other.CompareTag("Enemy"))
        {
            if (!hasCollided)
            {
                hasCollided = true; // ������������� ����
                rb.velocity = Vector2.zero; // ������������� ��������
                if (animator != null)
                {
                    animator.SetTrigger("Explode"); // ��������� ��������
                }
                else
                {
                    Destroy(gameObject); // �� ������ ���������� �������� ���������� ������
                }
            }
        }
    }

    // ���� ����� ���������� �� �������� � ����� (Animation Event)
    public void DestroyAfterExplosion()
    {
        Destroy(gameObject); // ���������� ������ ����� ��������� ��������
    }
}
