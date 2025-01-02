using UnityEngine;

/// <summary>
/// ������, ������������:
/// 1) ���������� ������� (Input) � Update()
/// 2) ������ ����������� "����� � �����" (OnCollisionEnter2D / OnCollisionStay2D)
/// 3) ����������� ����� � ����������� ��������
/// 4) ������ "���� ���� �� ������" (jumpWallHitUsed)
/// 5) ������� �������� ����� HealthManager
/// 6) ������ "lockedDirection", ����� ���������������� ��� �����/���������� ������ ������
/// </summary>
[RequireComponent(typeof(PlatformerPlayer))]
[RequireComponent(typeof(HealthManager))]
public class WallHitHandler : MonoBehaviour
{
    [Header("Wall Hit Settings")]
    public int wallHitThreshold = 5;      // �����, ��� ������� ��������� ����
    private int wallHitCount;             // ������� ������� ������

    // ���� ����� ������ ���� ���� �� ������, ���������� ���� ����
    private bool jumpWallHitUsed = false;

    // ����: �������� ������������� ��-�� ����� � �����
    private bool lockedByWallHit = false;

    // -1 = �������� "������" � ����� ����� (�������� ����� ������)
    // +1 = � ����� ������ (�������� ������)
    //  0 = ��� ���������� ��� �� ����������
    private int lockedDirection = 0;
    // ������ �� ������ ����������
    private PlatformerPlayer player;
    private HealthManager healthManager;

    // ------------------------------
    // ���� ��� "������ �����":
    // ------------------------------
    private bool pressLeftKeyDown;
    private bool pressRightKeyDown;
    private bool holdLeftKey;
    private bool holdRightKey;

    void Awake()
    {
        // �������� ������ �� ������������� �������
        player = GetComponent<PlatformerPlayer>();
        healthManager = GetComponent<HealthManager>();
    }

    void Update()
    {
        // ========= 1) ��������� ���� (����� �����) =========

        // ����������� ������� (�������� ����� ���� ����):
        pressLeftKeyDown = Input.GetKeyDown(KeyCode.LeftArrow);
        pressRightKeyDown = Input.GetKeyDown(KeyCode.RightArrow);

        // ��������� (��������, ���� ������ ������):
        holdLeftKey = Input.GetKey(KeyCode.LeftArrow);
        holdRightKey = Input.GetKey(KeyCode.RightArrow);

        // ========= 2) ������ "���� ��� �� ������" =========

        // ���� �������� ����������� � ��� � ������ � ���������� ����
        if (player.IsGrounded() && player.isJumping && jumpWallHitUsed)
        {
            player.isJumping = false;
            // �������� �������� (�������� ���):
            healthManager.anim.SetBool("IsGrounded", true);
            // ���� �����������, ���� �����
            player.landingSound.PlayLandingSound();
            jumpWallHitUsed = false;
        }

        // ========= 3) ������������� �������� �� ���������� "���" ������ =========
        if (lockedByWallHit)
        {
            // ���� "��������" �����
            if (lockedDirection == -1)
            {
                // ���� ������ ����� ������, ������� ����������������.
                // ��� ������ ��������� � � ��������������.
                if (!holdLeftKey)
                {
                    lockedByWallHit = false;
                    lockedDirection = 0;
                    player.isMovementLocked = false;
                }
            }
            // ���� "��������" ������
            else if (lockedDirection == +1)
            {
                if (!holdRightKey)
                {
                    lockedByWallHit = false;
                    lockedDirection = 0;
                    player.isMovementLocked = false;
                }
            }
            else
            {
                // ���� lockedDirection == 0 (������������ �� ������ ����),
                // �� ���� �����, ����� ��������������:
                lockedByWallHit = false;
                player.isMovementLocked = false;
            }
        }
    }

    // ����������� ��� ������ ����� � �������� �� ������
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Wall")) return;

        // ���������� ����� ��������
        foreach (ContactPoint2D contact in collision.contacts)
        {
            Vector2 normal = contact.normal;
            // ���������, ��� ����� ����� (����� �������������� �������)
            if (Mathf.Abs(normal.x) > 0.5f)
            {
                bool wallOnLeft = (normal.x > 0);
                bool wallOnRight = (normal.x < 0);

                // ������ �� ����� ��� ������?
                bool isPushingWall =
                    (wallOnLeft && holdLeftKey) ||
                    (wallOnRight && holdRightKey);

                if (isPushingWall)
                {
                    HandleWallCollision();
                    break;
                }
            }
        }
    }

    // ����������� ������ ����, ���� �������� ������� � �������� �� ������
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Wall")) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            Vector2 normal = contact.normal;
            if (Mathf.Abs(normal.x) > 0.5f)
            {
                bool wallOnLeft = (normal.x > 0);
                bool wallOnRight = (normal.x < 0);

                // ����� ������� (�����������) ��� ����������� �����
                bool isPushingWall =
                    (wallOnLeft && pressLeftKeyDown) ||
                    (wallOnRight && pressRightKeyDown);

                if (isPushingWall)
                {
                    HandleWallCollision();
                    break;
                }
            }
        }
    }

    /// <summary>
    /// ����������, ���� ����������, ��� ����� "���������" � �����
    /// (��� ����� ��� ��� ����� �������).
    /// </summary>
    private void HandleWallCollision()
    {
        if (player.isJumping)
        {
            // ���� ���� �� ������
            if (!jumpWallHitUsed)
            {
                jumpWallHitUsed = true;
                IncrementWallHit();
            }
        }
        else
        {
            IncrementWallHit();
        }
    }

    /// <summary>
    /// ����������� ������� "������ � �����", ������� ���� ��� ���������� ������
    /// � ��������� �������� �� ���������� "���" ������, ������� ������� ����.
    /// </summary>
    private void IncrementWallHit()
    {
        wallHitCount++;
        Debug.Log($"WallHitCount = {wallHitCount}");

        // --- ����������, ����� �������� �� ��������� --- 
        // ���� holdLeftKey == true, ������ ���� �����; ���� holdRightKey == true, ������ ������
        if (holdLeftKey && !holdRightKey)
        {
            lockedDirection = -1; // ���������, ��������� �����
        }
        else if (holdRightKey && !holdLeftKey)
        {
            lockedDirection = +1; // ���������� ������
        }
        else
        {
            // ���� ������ ��� ������ ��� ���-�� ��������, ����� �����
            // ������� ����� �� OnCollisionStay, �� ��� ������� ��������:
            lockedDirection = 0;
        }

        // ��������� ��������
        lockedByWallHit = true;
        player.isMovementLocked = true;

        // ���� �������� ������ � ������� ����
        if (wallHitCount >= wallHitThreshold)
        {
            wallHitCount = 0; // ���������� �������

            healthManager.Starthealth -= 1;
            healthManager.numberStringDisplay.SetDoubleDigitNumber(healthManager.Starthealth);

            // ���� �������� ����
            if (!healthManager.isDead && healthManager.Starthealth <= 0)
            {
                healthManager.Die();
                healthManager.isDead = true;
                player.body.velocity = Vector2.zero;
            }
            else
            {
                // ������ �������� ����� (���� �� �������)
                if (!player.isJumping)
                {
                    if (player.isCrouching)
                        healthManager.anim.SetTrigger("TakeDamageCrouching");
                    else
                        healthManager.anim.SetTrigger("TakeDamageStanding");
                }
                healthManager.PlayDamageSound();
            }
        }
        else
        {
            // ���� ����� �� ��������� � "�����" �������� �����
            if (!player.isJumping)
            {
                if (player.isCrouching)
                    healthManager.anim.SetTrigger("TakeDamageCrouching");
                else
                    healthManager.anim.SetTrigger("TakeDamageStanding");
            }
            healthManager.PlayDamageSound();
        }
    }
}
