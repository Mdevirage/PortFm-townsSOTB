using UnityEngine;

public class LandingSound : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip defaultlandingSound;
    public AudioClip WoodlandingSound;
    private AudioClip currentlandingSound; // ������� ����� ������ �����
    private string currentSurfaceTag = "Ground"; // ������� ��� �����������

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // ����� ��� ��������������� ����� �����������
    public void PlayLandingSound()
    {
        if (audioSource != null && currentlandingSound != null)
        {
            audioSource.PlayOneShot(currentlandingSound);
        }
    }
    public void UpdateSurfaceTag(string tag)
    {
        currentSurfaceTag = tag;

        switch (currentSurfaceTag)
        {
            case "Wood":
                currentlandingSound = WoodlandingSound;
                break;
            default:
                currentlandingSound = defaultlandingSound;
                break;
        }
    }

    public void ResetSurfaceTag(string tag)
    {
        if (tag == "Ground")
        {
            currentSurfaceTag = "Ground";
            currentlandingSound = defaultlandingSound;
        }
    }
}
