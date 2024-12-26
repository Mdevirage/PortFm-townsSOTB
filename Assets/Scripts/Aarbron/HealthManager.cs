using System.Collections;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [Header("General Health")]
    public int Starthealth = 24;              // ���������� ��������
    public NumberStringDisplay numberStringDisplay; // ����������� ��������
    public Animator anim;                     // �������� ��� ������������ �������� �����/������

    private PlatformerPlayer playerCode;       // ������ �� ������ ���������
    private LadderMovement ladderCode;         // ������ �� ������� �� �������� (���� �����)
    private CombatSystem combatSystem;         // ������ �� ������� �����

    [Header("Wall Hit Settings")]
    public int wallHitCount = 0;              // ������� ����� ������������ �� ������
    public int wallHitThreshold = 5;          // �����, ��� ������� �������� ������� �������� ����
    public float wallHitCooldown = 0.5f;      // ������� ��� ����� ��������� ������ �� �����
    private float lastWallHitTime;            // ����� ��������� ��� ����������� ����?
    public bool hasJumpCollision = false;

    [Header("Invincibility Settings")]
    public bool isDead = false;               // ���� ��������� ������
    public bool isInvincible = false;         // ���� ��������� ������������
    public float invincibilityDuration = 2f;  // ������������ ������������

    [Header("Audio Settings")]
    public AudioClip[] damageSounds;          // ������ ������ ��� ��������� �����
    private AudioSource audioSource;          // ��������� ��� ��������������� ������

    public bool getdamage = false;            // ���� ����� ���-�� ��������� �����
    public bool button = false;
    void Start()
    {
        playerCode = GetComponent<PlatformerPlayer>();
        ladderCode = GetComponent<LadderMovement>();
        anim = GetComponent<Animator>();
        combatSystem = GetComponent<CombatSystem>();
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);

        audioSource = gameObject.AddComponent<AudioSource>(); // �������������� AudioSource
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        // ���������, �������� �� ������ ������������� �������
        if (!collision.gameObject.CompareTag("Wall"))
            return;

        // ���������, ��������� �� �������� � ������
        if (playerCode.isJumping && hasJumpCollision)
        {
            return; // ���� ��� ���� ��������� ������������, �� ����������� �������
        }

        foreach (ContactPoint2D contact in collision.contacts)
        {
            Vector2 normal = contact.normal;

            // ��������, ��� ��� ����� �������������� ������������:
            if (Mathf.Abs(normal.x) > 0.5f)
            {
                bool wallOnLeft = (normal.x > 0f);
                bool wallOnRight = (normal.x < 0f);

                if (IsPressingForwardIntoWall(wallOnLeft, wallOnRight))
                {
                    AddWallHit();

                    if (playerCode.isJumping)
                    {
                        hasJumpCollision = true; // ������������� ���� ������������ � ������
                    }

                    break;
                }
            }
        }
    }

    /// <summary>
    /// ���������� true, ���� ����� ��� ������ "�����" ����� � �� �������, ��� �����.
    /// ��������, ���� ����� ������, � ����� ����� Horizontal > 0,
    /// ��� ������ ������ �����.
    /// </summary>
    bool IsPressingForwardIntoWall(bool wallOnLeft, bool wallOnRight)
    {
        float inputX = 0;
        
        // ���������, ����� �� ������������ ������ � ���� �����:
        if (Input.GetKey(KeyCode.LeftArrow) && !button)
        {
            inputX = -1;
            // ������ ������������� ������� �����
        }
        else if (Input.GetKey(KeyCode.RightArrow) && !button)
        {
            inputX = 1;
            // ������ ������������� ������� ������
        }
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            button = false;
        }

        // ���� ����� �����, ������ "�����" = inputX < 0
        // ���� ����� ������, ������ "�����" = inputX > 0
        if (wallOnRight && inputX > 0) 
        {
            button = true;
            return true;
        }

        if (wallOnLeft && inputX < 0)
        {
            button = true;
            return true;
        }
        return false;
    }
    public void AddWallHit()
    {
        // ��������� �������
        if (Time.time - lastWallHitTime < wallHitCooldown)
            return;
        lastWallHitTime = Time.time;

        // ����������� �������
        wallHitCount++;
        Debug.Log($"WallHitCount = {wallHitCount}");

        // ���� ��������� ����� � �������� ����
        if (wallHitCount >= wallHitThreshold)
        {
            wallHitCount = 0; // ���������� �������
            Starthealth -= 1;
            numberStringDisplay.SetDoubleDigitNumber(Starthealth);

            if (!isDead && Starthealth <= 0)
            {
                Die();
                isDead = true;
                playerCode.body.velocity = Vector2.zero;
            }
        }

        if (playerCode.isTurning)
        {
            playerCode.isTurning = false;
            playerCode.previousDirection = transform.localScale.x;
            playerCode.isCrouching = false;
        }
        if (playerCode.isMovementLocked)
        {
            playerCode.isMovementLocked = false;
        }
        if (!ladderCode.isClimbing && !playerCode.isFalling && !playerCode.isJumping && !isDead)
        {
            if (playerCode.isCrouching)
            {
                anim.SetTrigger("TakeDamageCrouching");
            }
            else
            {
                anim.SetTrigger("TakeDamageStanding");
            }
        }
        PlayDamageSound();
    }

    /// <summary>
    /// �������� ���� (����� 1 ��������). ������ �� ������ ����.
    /// </summary>
    public void TakeDamage()
    {
        if (isDead || isInvincible) return; // �� �������� ����, ���� ������ ��� ���������

        Starthealth -= 1;
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);

        // ���������� �����, ���� �� ����� ��������� ����� �������� �������
        combatSystem.ResetAttack();
        combatSystem.isAttackingReverse = false;

        // ���� �������� ����� ���� 0 � ������
        if (!isDead && Starthealth <= 0)
        {
            Die();
            isDead = true;
            playerCode.body.velocity = Vector2.zero;
        }

        // ���� ��� ��� ����
        if (Starthealth > 0)
        {
            // ������ �������� / ���������
            StartCoroutine(numberStringDisplay.BlinkEffect());

            // �������� ��������� ������������
            StartCoroutine(InvincibilityCoroutine());
        }

        // ������� ����� �� ������, ���� �������� ��� � ������ ���������
        if (playerCode.isTurning)
        {
            playerCode.isTurning = false;
            playerCode.previousDirection = transform.localScale.x;
            playerCode.isCrouching = false;
        }
        if (playerCode.isMovementLocked)
        {
            playerCode.isMovementLocked = false;
        }

        // ��������� �������� ��������� �����,
        // �� ������ ���� �� �� ��������, �� � ������� � �� ������, � ��� ���� ����
        if (!ladderCode.isClimbing && !playerCode.isFalling && !playerCode.isJumping && !isDead)
        {
            if (playerCode.isCrouching)
            {
                anim.SetTrigger("TakeDamageCrouching");
            }
            else
            {
                anim.SetTrigger("TakeDamageStanding");
            }
        }

        // ��������� ���� ��������� �����
        PlayDamageSound();
    }

    private void PlayDamageSound()
    {
        if (damageSounds.Length > 0)
        {
            int index = Random.Range(0, damageSounds.Length);
            audioSource.PlayOneShot(damageSounds[index]);
        }
    }

    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    public void Kill()
    {
        if (isDead) return;
        Starthealth = 0;
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);
        PlayDamageSound();
        Die();

    }

    public void Die()
    {
        anim.SetTrigger("DeathTrigger");
    }

    public void OnDeathAnimationComplete()
    {
        gameObject.SetActive(false);
    }

    public void DamageReset()
    {
        combatSystem.ResetAttack();
        playerCode.isTurning = false;
    }
}
