using UnityEngine;

public class LadderStepSound : MonoBehaviour
{
    public AudioClip[] ladderStepSounds;  // Массив звуков шагов на лестнице
    private AudioSource audioSource;
    private int stepIndex = 0;           // Индекс текущего звука

    void Start()
    {
        audioSource = GetComponent<AudioSource>();  // Получаем компонент AudioSource
    }

    // Метод вызывается через Animation Event
    public void PlayLadderStepSound()
    {
        if (audioSource != null && ladderStepSounds.Length > 0)
        {
            // Воспроизводим текущий звук
            audioSource.PlayOneShot(ladderStepSounds[stepIndex]);

            // Переходим к следующему звуку в массиве
            stepIndex = (stepIndex + 1) % ladderStepSounds.Length;
        }
    }
}
