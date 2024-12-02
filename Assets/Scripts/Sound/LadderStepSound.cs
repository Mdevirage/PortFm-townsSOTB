using UnityEngine;

public class LadderStepSound : MonoBehaviour
{
    public AudioClip[] ladderStepSounds;  // ������ ������ ����� �� ��������
    private AudioSource audioSource;
    private int stepIndex = 0;           // ������ �������� �����

    void Start()
    {
        audioSource = GetComponent<AudioSource>();  // �������� ��������� AudioSource
    }

    // ����� ���������� ����� Animation Event
    public void PlayLadderStepSound()
    {
        if (audioSource != null && ladderStepSounds.Length > 0)
        {
            // ������������� ������� ����
            audioSource.PlayOneShot(ladderStepSounds[stepIndex]);

            // ��������� � ���������� ����� � �������
            stepIndex = (stepIndex + 1) % ladderStepSounds.Length;
        }
    }
}
