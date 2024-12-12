using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    [Header("Footstep Sounds")]
    public AudioClip[] defaultFootstepSounds; // Звуки шагов по стандартной поверхности
    public AudioClip[] WoodFootstepSounds;   // Звуки шагов по дереву
    public AudioClip[] stoneFootstepSounds;  // Звуки шагов по камню

    private AudioSource audioSource;
    private Animator anim;
    private AudioClip[] currentFootstepSounds; // Текущий набор звуков шагов
    private int stepIndex = 0;                 // Индекс для отслеживания текущего звука
    private string currentSurfaceTag = "Ground"; // Текущий тег поверхности

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        currentFootstepSounds = defaultFootstepSounds; // По умолчанию используем стандартные звуки шагов
    }

    void Update()
    {
        // Сбрасываем звуки шагов, если персонаж перестал двигаться
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
