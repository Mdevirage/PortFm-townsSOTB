using System.Collections;
using UnityEngine;

/// <summary>
/// ������ �������, ������� ������������ ������������ �� ������,
/// ���������, ��� �� �� ������ ������ � ������� �����,
/// � ��� ������������� �������� ������, ������������� ������� �wallHitCount�.
///
/// �������� ���������� � ���� � HealthManager (��� ��������), � CollisionHandler.
/// � �������� ������� ����� ��������� �� 2-3 �������.
/// </summary>
public class WallCollisionHandler : MonoBehaviour
{
    /*[Header("WALL SETTINGS")]
    [Tooltip("Tag, ������� �� ������� '������'")]
    public string wallTag = "Wall";

    [Tooltip("�������� (������) ����� '�������' �� �����, ���� �������� ����� � ���")]
    public float wallHitInterval = 0.5f;
    private float nextWallHitTime = 0f;

    [Header("WALL HIT COUNTER")]
    [Tooltip("������� '������-������' �����, ����� ������� �������� ����?")]
    public int wallHitThreshold = 5;
    [Tooltip("������� ������� ������-������ �� �����.")]
    public int wallHitCount = 0;

    [Header("HEALTH SETTINGS")]
    public int startHealth = 20;            // ��������
    public bool isInvincible = false;       // ��������� ������������, ���� �����
    public bool isDead = false;
    [Tooltip("�� ������� ������ �������� ���������� �������� ����� ����� (������).")]
    public float invincibilityDuration = 1.5f;

    // ������ �� Animator (��� ������������ �������� �����/������, ���� �����)
    private Animator anim;
    private Rigidbody2D rb;

    void Start()
    {
        // �����������, ��� �� ���� �� ������� ���� Animator, Rigidbody2D
        // ��� �� �������� ������ ����� GetComponentInChildren<Animator>() � �. �.
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // ���� �������� ��� ���� � ������ �� ������
        if (isDead) return;

        // ���� �����, ����� ������� ��������� ��� ������, ����� ����� ��������� ��� ������.

        // � OnCollisionStay2D �� *����������* ������������, �� ��� ��������� ��������
        // ����� ������ ��� ��. ��� ����� �������� ������ OnCollisionStay2D -> AttemptWallHit().
        // ������ ���� �� ������ ��������� "��� � N ������" (cooldown) ����� � OnCollisionStay2D,
        // ������� ���.  
        // � ������ ������� �� ������� ��� *�����* � OnCollisionStay2D
        // (��. ����� ����).
    }

    // -----------------------------------------------------------------------
    //    ������ ������������: OnCollisionStay2D, ������ ��������
    // -----------------------------------------------------------------------
    void OnCollisionStay2D(Collision2D collision)
    {
        // ��������� Tag
        if (!collision.gameObject.CompareTag(wallTag)) return;

        // ���������� ����� ���������������
        foreach (ContactPoint2D contact in collision.contacts)
        {
            Vector2 normal = contact.normal;
            // ���� ������� ������ �������� �������������� (��� ����� ��������� �����������),
            // ������ ��� ������� ��������, � �� ���/�������.
            if (Mathf.Abs(normal.x) > 0.5f)
            {
                // normal.x > 0 => ����� ����� (�.�. ������� ������� ������)
                // normal.x < 0 => ����� ������ (�.�. ������� ������� �����)

                bool wallIsOnLeft = (normal.x > 0f);
                bool wallIsOnRight = (normal.x < 0f);

                // �����, ��� �� �� "�����"
                if (IsPressingForward(wallIsOnRight, wallIsOnLeft))
                {
                    AttemptWallHit();
                    // ��� ����� ���������� ������� � ����� �������� �� �����
                    // (����� �� ������� ��������� ��� �� ���� Stay)
                    break;
                }
            }
        }
    }

    /// <summary>
    /// ��������: ��� �� �� ������ ����� � ������� �����?
    /// ��������, ���� ����� ������, � ����� ��� "������".
    /// </summary>
    private bool IsPressingForward(bool wallOnRight, bool wallOnLeft)
    {
        float inputX = Input.GetAxisRaw("Horizontal");

        // ���� ����� ������, ������ "�������� �����" = (inputX > 0).
        // ���� ����� �����, ������ "�������� �����" = (inputX < 0).

        if (wallOnRight && inputX > 0)
            return true;
        if (wallOnLeft && inputX < 0)
            return true;

        return false;
    }

    /// <summary>
    /// ������: ����� �� "�����" � �����, ��� � wallHitInterval ������
    /// �������������� ������� wallHitCount.
    /// </summary>
    private void AttemptWallHit()
    {
        // ��������� �������
        if (Time.time < nextWallHitTime) return;

        // ��������� ����� ����. �����
        nextWallHitTime = Time.time + wallHitInterval;

        // ����������� �������
        wallHitCount++;
        Debug.Log($"[WallCollision] wallHitCount = {wallHitCount}");

        // ���������, �� ��������� �� ����� ��� ��������� �����
        if (wallHitCount >= wallHitThreshold)
        {
            wallHitCount = 0;    // ���������� �������
            TakeDamageWall();    // ������� �������� ����
        }
    }

    // -----------------------------------------------------------------------
    //    ������ ����� / ��������
    // -----------------------------------------------------------------------
    private void TakeDamageWall()
    {
        // ���� ��� ���� ��� �������� � �� �������� ����.
        if (isDead || isInvincible) return;

        // ��������� ��������
        startHealth--;
        Debug.Log($"TakeDamage from wall: health = {startHealth}");

        // ���� �������� ��������� � Die()
        if (startHealth <= 0)
        {
            isDead = true;
            anim?.SetTrigger("DeathTrigger");
            // ����� ��������� ����������, ������� gameOver � �. �.
            return;
        }

        // ����� � ��������� ������������ (����� �� ������� �� ����� ������������ ������)
        StartCoroutine(InvincibilityRoutine());

        // ��������� �������� "������� ����", ���� ������
        /*if (anim != null)
        {
            anim.SetTrigger("TakeDamageStanding");
        }
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    // ���� ������ ���������� ������ ���������� �������� ������:
    // ��������� �� Animation Event:
    public void OnDeathAnimationComplete()
    {
        // gameObject.SetActive(false);
        Destroy(gameObject);
    }*/
}
