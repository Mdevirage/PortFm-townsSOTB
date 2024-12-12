using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    [Header("Footstep Sounds")]
    public AudioClip[] defaultFootstepSounds; // ����� ����� �� ����������� �����������
    public AudioClip[] WoodFootstepSounds;   // ����� ����� �� ������
    public AudioClip[] stoneFootstepSounds;  // ����� ����� �� �����

    private AudioSource audioSource;
    private Animator anim;
    private AudioClip[] currentFootstepSounds; // ������� ����� ������ �����
    private int stepIndex = 0;                 // ������ ��� ������������ �������� �����
    private string currentSurfaceTag = "Ground"; // ������� ��� �����������

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        currentFootstepSounds = defaultFootstepSounds; // �� ��������� ���������� ����������� ����� �����
    }

    void Update()
    {
        // ���������� ����� �����, ���� �������� �������� ���������
        if (anim.GetFloat("Speed") == 0 || anim.GetCurrentAnimatorStateInfo(0).IsName("Aarbron_Jump"))
        {
            ResetFootstepSound();
        }
    }

    public void PlayFootstepSound()
    {
        if (audioSource != null && currentFootstepSounds.Length > 0)
        {
            audioSource.PlayOneShot(currentFootstepSounds[stepIndex]);
            stepIndex = (stepIndex + 1) % currentFootstepSounds.Length;
        }
    }

    private void ResetFootstepSound()
    {
        stepIndex = 0;
    }

    public void UpdateSurfaceTag(string tag)
    {
        currentSurfaceTag = tag;

        switch (currentSurfaceTag)
        {
            case "Wood":
                currentFootstepSounds = WoodFootstepSounds;
                break;
            default:
                currentFootstepSounds = defaultFootstepSounds;
                break;
        }
    }

    public void ResetSurfaceTag(string tag)
    {
        if (tag == currentSurfaceTag)
        {
            currentSurfaceTag = "Ground";
            currentFootstepSounds = defaultFootstepSounds;
        }
    }
}
