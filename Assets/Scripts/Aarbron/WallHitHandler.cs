using UnityEngine;

[RequireComponent(typeof(PlatformerPlayer))]
[RequireComponent(typeof(HealthManager))]
public class WallHitHandler : MonoBehaviour
{
    [Header("Wall Hit Settings")]
    public int wallHitThreshold = 5;      // ����� ��� ���������� �����
    private int wallHitCount;             // ������� ������� ������

    // ���� ��������� ������ ���� ���� �� ������, ���������� ���� ����
    private bool jumpWallHitUsed = false;

    // ����: �������� ������������� ��-�� ����� � �����
    private bool lockedByWallHit = false;

    // -1 = �������� "������" � ����� ����� (���������� ����� ������)
    // +1 = � ������ ����� (���������� ������)
    //  0 = ��� ���������� ��� �� ����������
    private int lockedDirection = 0;

    // ������ �� ������ ����������
    private PlatformerPlayer player;
    private HealthManager healthManager;

    // ���� ��� "������ �����"
    private bool pressLeftKeyDown;
    private bool pressRightKeyDown;
    private bool holdLeftKey;
    private bool holdRightKey;

    [Header("Wall Detection Settings")]
    public float wallCheckDistance = 0.1f; // ��������� ��� �������� ����
    public LayerMask wallLayerMask;        // ����� ���� ��� ������������� ����
    public Vector2 wallCheckOffsetLeft = new Vector2(-0.5f, 0f);  // �������� ��� �������������� raycast
    public Vector2 wallCheckOffsetRight = new Vector2(0.5f, 0f);  // �������� ��� ��������������� raycast

    // ����������� ����� ��� ������������ ������������ ������������
    private bool leftWallHitProcessed = false;
    private bool rightWallHitProcessed = false;

    void Awake()
    {
        // �������� ������ �� ������������� �������
        player = GetComponent<PlatformerPlayer>();
        healthManager = GetComponent<HealthManager>();
    }

    void FixedUpdate()
    {
        // ========= 1) ��������� ���� (����� �����) =========
        // ���������� Input � Update(), �� ��������� ���������� ���������� � FixedUpdate, ����� ���������� ����

        // ����������� �������
        pressLeftKeyDown = Input.GetButtonDown("Left");
        pressRightKeyDown = Input.GetButtonDown("Right");

        // ��������� ������
        holdLeftKey = Input.GetButton("Left");
        holdRightKey = Input.GetButton("Right");

        // ========= 2) ��������� ������ ������ =========
        if (player.IsGrounded() && player.isJumping && jumpWallHitUsed)
        {
            player.isJumping = false;
            healthManager.anim.SetBool("IsGrounded", true);
            player.landingSound.PlayLandingSound();
            jumpWallHitUsed = false;
        }

        // ========= 3) ������������� ��������, ���� ���������� =========
        HandleMovementUnlock();

        // ========= 4) ���������� ����������� ���� =========
        DetectWalls();
    }

    /// <summary>
    /// ������������ ����� � ����� ������ � ������� raycasts.
    /// </summary>
    private void DetectWalls()
    {
        // ���������� �������� ����� ��� raycasts �� ������ ������� ������
        Vector2 originLeft = (Vector2)transform.position + wallCheckOffsetLeft;
        Vector2 originRight = (Vector2)transform.position + wallCheckOffsetRight;

        // ��������� raycasts ����� � ������
        RaycastHit2D hitLeft = Physics2D.Raycast(originLeft, Vector2.left, wallCheckDistance, wallLayerMask);
        RaycastHit2D hitRight = Physics2D.Raycast(originRight, Vector2.right, wallCheckDistance, wallLayerMask);

        // ���������� ���� (�����������, ��� ������������)
        Debug.DrawRay(originLeft, Vector2.left * wallCheckDistance, Color.red);
        Debug.DrawRay(originRight, Vector2.right * wallCheckDistance, Color.blue);

        bool wallOnLeft = hitLeft.collider != null;
        bool wallOnRight = hitRight.collider != null;

        // ���������, ���������� �� ����� ������ ������ ����� � �� ���� �� ��� ���������� ������������
        bool isPushingWallLeft = wallOnLeft && holdLeftKey && !leftWallHitProcessed;
        bool isPushingWallRight = wallOnRight && holdRightKey && !rightWallHitProcessed;

        if (isPushingWallLeft || isPushingWallRight)
        {
            if (isPushingWallLeft)
            {
                HandleWallCollision(-1);
                leftWallHitProcessed = true; // ������������� ����
            }
            if (isPushingWallRight)
            {
                HandleWallCollision(+1);
                rightWallHitProcessed = true; // ������������� ����
            }
        }
    }

    /// <summary>
    /// ������������ ������ ������������ �� ������ �� ������ �����������.
    /// </summary>
    /// <param name="direction">-1 ��� �����, +1 ��� ������ �������</param>
    private void HandleWallCollision(int direction)
    {
        if (player.isJumping)
        {
            // ���� ���� �� ������
            if (!jumpWallHitUsed)
            {
                jumpWallHitUsed = true;
                IncrementWallHit(direction);
            }
        }
        else
        {
            IncrementWallHit(direction);
        }
    }

    /// <summary>
    /// ����������� ������� ������ � �����, ������� ���� ��� ���������� ������ � ��������� ��������.
    /// </summary>
    /// <param name="direction">����������� ����� � ����� (-1 ��� +1)</param>
    private void IncrementWallHit(int direction)
    {
        wallHitCount++;
        Debug.Log($"WallHitCount = {wallHitCount}");
        player.EndJump();
        // ��������� ��������
        lockedByWallHit = true;
        player.isMovementLocked = true;

        // ���������� ����������� ���������� �� ������ �����
        lockedDirection = direction;

        if (player.isTurning)
        {
            player.isTurning = false;
            player.previousDirection = transform.localScale.x;
            player.isCrouching = false;
        }

        // ��������� ����, ���� ��������� �����
        if (wallHitCount >= wallHitThreshold)
        {
            wallHitCount = 0; // ���������� �������

            healthManager.Starthealth -= 1;
            healthManager.numberStringDisplay.SetDoubleDigitNumber(healthManager.Starthealth);

            // ���������, ���� �� ��������
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
            // ������ ����� �������� �����
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

    /// <summary>
    /// ������������ �������� ������, ����� ��������������� ������ ��������.
    /// </summary>
    private void HandleMovementUnlock()
    {
        if (lockedByWallHit)
        {
            if (lockedDirection == -1 && !holdLeftKey)
            {
                UnlockMovement();
                leftWallHitProcessed = false; // ���������� ���� ��� ����� �������
            }
            else if (lockedDirection == +1 && !holdRightKey)
            {
                UnlockMovement();
                rightWallHitProcessed = false; // ���������� ���� ��� ������ �������
            }
            else if (lockedDirection == 0)
            {
                // ��������� ������: ��������������, ���� ����������� ������������
                UnlockMovement();
                leftWallHitProcessed = false;
                rightWallHitProcessed = false;
            }
        }
    }

    /// <summary>
    /// ������������ �������� ������.
    /// </summary>
    private void UnlockMovement()
    {
        lockedByWallHit = false;
        lockedDirection = 0;
        player.isMovementLocked = false;
    }

    void Update()
    {
        // ��������� �����, ������� ���������� ��������� ������ ����
        // ��� ������������� ����� ����������� ��������� ����� ���� ������ FixedUpdate
    }
}
