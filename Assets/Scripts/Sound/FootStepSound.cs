using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    public AudioClip[] footstepSounds;  // ������ ������ �����
    private AudioSource audioSource;
    private int stepIndex = 0;          // ������ ��� ������������ �������� ����;
    private Animator anim;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();  // �������� ��������� AudioSource
        anim = GetComponent<Animator>();            // �������� ��������� Animator
    }

    // ���� ����� ����� ���������� ����� Animation Event
    public void PlayFootstepSound()
    {
        if (audioSource != null && footstepSounds.Length > 0)
        {
            audioSource.PlayOneShot(footstepSounds[stepIndex]);  // ��������������� �������� �����
            stepIndex = (stepIndex + 1) % footstepSounds.Length;  // ������� � ���������� �����
        }
    }

    // ����� ��� ������ ������� �����
    public void ResetFootstepSound()
    {
        stepIndex = 0;  // ����� ������� ����� �� ������ ����
    }

    // ���� ����� ���������� � Update, ����� ���������, ���� �������� ������������
    void Update()
    {
        // ���������, ���� �������� ������ �� ��������� �������� "Run" (��� ������ �������� ��������)
        if (anim.GetFloat("Speed") == 0 || anim.GetCurrentAnimatorStateInfo(0).IsName("Aarbron_Jump"))  // ��������, ������������ �������� "Speed" ����� ���� ��� ���������
        {
            ResetFootstepSound();  // ���� �������� ������������, ���������� ������ ����� �� ������
        }
    }
}
