using UnityEngine;

public class LandingSound : MonoBehaviour
{
    public AudioClip landingSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // ћетод дл€ воспроизведени€ звука приземлени€
    public void PlayLandingSound()
    {
        if (audioSource != null && landingSound != null)
        {
            audioSource.PlayOneShot(landingSound);
        }
    }
}
