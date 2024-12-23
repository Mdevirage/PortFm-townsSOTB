using UnityEditor;
using UnityEngine;
public class RandomAudioPlayer : MonoBehaviour
{
    [Header("Random Clips")]
    public AudioClip[] possibleClips; // Массив аудиоклипов, один из которых выбирается случайно

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        // Если есть хотя бы один клип
        if (possibleClips != null && possibleClips.Length > 0)
        {
            // Выбираем случайный индекс
            int randomIndex = Random.Range(0, possibleClips.Length);

            // Подставляем его в AudioSource
            audioSource.clip = possibleClips[randomIndex];

            // Проигрываем звук
            audioSource.Play();
        }
    }
    public void DestroyExplosion()
    {
        Destroy(gameObject);
    }
}
