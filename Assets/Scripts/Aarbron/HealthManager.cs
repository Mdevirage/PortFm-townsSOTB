using System.Collections;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public int Starthealth = 24; // ���������� ��������
    public NumberStringDisplay numberStringDisplay; // ����������� ��������
    public Animator anim;
    private PlatformerPlayer playerCode;
    private LadderMovement ladderCode;
    private CombatSystem combatSystem;

    public bool isDead = false; // ���� ��������� ������
    public bool isInvincible = false; // ���� ��������� ������������
    public float invincibilityDuration = 2f; // ������������ ������������

    [Header("Audio Settings")]
    public AudioClip[] damageSounds; // ������ ������ ��� ��������� �����
    private AudioSource audioSource; // ��������� ��� ��������������� ������

    public bool getdamage =false;
    void Start()
    {
        playerCode = GetComponent<PlatformerPlayer>();
        ladderCode = GetComponent<LadderMovement>();
        anim = GetComponent<Animator>();
        combatSystem = GetComponent<CombatSystem>();
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);

        // �������������� AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
    }
    public void TakeDamage()
    {
        if (isDead || isInvincible) return; // �� �������� ����, ���� ������ ��� ���������

        
        Starthealth -= 1;
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);
        //getdamage = true;
        combatSystem.ResetAttack(); // ���������� ����� ��� ��������� �����
        combatSystem.isAttackingReverse = false;
        if (!isDead && Starthealth - 1 < 0)
        {
            Die();
            isDead = true;
            playerCode.body.velocity = Vector2.zero;
        }
        if (Starthealth > 0)
        {
            StartCoroutine(numberStringDisplay.BlinkEffect());
            StartCoroutine(InvincibilityCoroutine()); // ��������� ��������� ������������
        }
        // ���� �������� ��������������, ������� ��� ���������
        if (playerCode.isTurning)
        {
            playerCode.isTurning = false;
            playerCode.previousDirection = transform.localScale.x;
            playerCode.isCrouching = false;
        }
        if (playerCode.isMovementLocked) { 
            playerCode.isMovementLocked = false;
        }
        // ��������� �������� ��������� �����
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

            PlayDamageSound(); // ������������� ���� �����
        }
    }

    private void PlayDamageSound()
    {
        if (damageSounds.Length > 0) // ���������, ���� �� ����� � �������
        {
            int index = Random.Range(0, damageSounds.Length); // �������� ��������� ����
            audioSource.PlayOneShot(damageSounds[index]); // ����������� ����
        }
    }

    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true; // �������� ������������
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false; // ��������� ������������
    }

    public void Kill()
    {
        if (isDead) return;

        Starthealth = 0;
        numberStringDisplay.SetDoubleDigitNumber(Starthealth);
        Die();
    }

    public void Die()
    {
        anim.SetTrigger("DeathTrigger"); // ��������� �������� ������
    }

    public void OnDeathAnimationComplete()
    {
        //Debug.Log("Animation Event: Death animation finished.");
        gameObject.SetActive(false);
    }
    public void DamageReset() 
    {
        combatSystem.ResetAttack();
        playerCode.isTurning = false;

    }
}
