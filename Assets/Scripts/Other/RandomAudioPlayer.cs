using UnityEditor;
using UnityEngine;
public class RandomAudioPlayer : MonoBehaviour
{
    [Header("Random Clips")]
    public AudioClip[] possibleClips; // ������ �����������, ���� �� ������� ���������� ��������

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        // ���� ���� ���� �� ���� ����
        if (possibleClips != null && possibleClips.Length > 0)
        {
            // �������� ��������� ������
            int randomIndex = Random.Range(0, possibleClips.Length);

            // ����������� ��� � AudioSource
            audioSource.clip = possibleClips[randomIndex];

            // ����������� ����
            audioSource.Play();
        }
    }
    public void DestroyExplosion()
    {
        Destroy(gameObject);
    }
}
