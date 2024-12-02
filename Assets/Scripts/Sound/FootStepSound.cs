using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    public AudioClip[] footstepSounds;  // Массив звуков шагов
    private AudioSource audioSource;
    private int stepIndex = 0;          // Индекс для отслеживания текущего звук;
    private Animator anim;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();  // Получаем компонент AudioSource
        anim = GetComponent<Animator>();            // Получаем компонент Animator
    }

    // Этот метод будет вызываться через Animation Event
    public void PlayFootstepSound()
    {
        if (audioSource != null && footstepSounds.Length > 0)
        {
            audioSource.PlayOneShot(footstepSounds[stepIndex]);  // Воспроизведение текущего звука
            stepIndex = (stepIndex + 1) % footstepSounds.Length;  // Переход к следующему звуку
        }
    }

    // Метод для сброса индекса звука
    public void ResetFootstepSound()
    {
        stepIndex = 0;  // Сброс индекса шагов на первый звук
    }

    // Этот метод вызывается в Update, чтобы проверять, если анимация остановилась
    void Update()
    {
        // Проверяем, если персонаж больше не выполняет анимацию "Run" (или другую анимацию движения)
        if (anim.GetFloat("Speed") == 0 || anim.GetCurrentAnimatorStateInfo(0).IsName("Aarbron_Jump"))  // Например, анимационный параметр "Speed" равен нулю при остановке
        {
            ResetFootstepSound();  // Если анимация остановилась, сбрасываем индекс звука на первый
        }
    }
}
