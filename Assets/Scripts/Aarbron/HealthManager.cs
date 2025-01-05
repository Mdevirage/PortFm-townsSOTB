using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour
{
    [Header("General Health")]
    public int Starthealth = 24;              // ���������� ��������
    public NumberStringDisplay numberStringDisplay; // ����������� ��������
    public Animator anim;                     // �������� ��� ������������ �������� �����/������

    private PlatformerPlayer playerCode;       // ������ �� ������ ���������
    private LadderMovement ladderCode;         // ������ �� ������� �� �������� (���� �����)
    private CombatSystem combatSystem;         // ������ �� ������� �����

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
    public void TakeDamageBoss()
    {
        if (isDead || isInvincible) return; // �� �������� ����, ���� ������ ��� ���������

        Starthealth -= 1;
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);

        // ���� �������� ����� ���� 0 � ������
        if (!isDead && Starthealth <= 0)
        {
            isDead = true;
            Die();
            playerCode.body.velocity = Vector2.zero;
        }
    }
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
            isDead = true;
            Die();
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

    public void HPPotion()
    {
        Starthealth = 24;
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);
    }
    public void PlayDamageSound()
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
        isDead = true;
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
        SceneManager.LoadScene("DeathScene"); // ������������ �� ����� � �����
        gameObject.SetActive(false);
    }

    public void DamageReset()
    {
        combatSystem.ResetAttack();
        playerCode.isTurning = false;
    }
}
