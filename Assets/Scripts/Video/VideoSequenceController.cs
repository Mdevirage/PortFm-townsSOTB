using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VideoSequenceController : MonoBehaviour
{
    private VideoPlayer[] videoPlayers; // ������ ��� �������� VideoPlayer
    public string nextSceneName; // ��� ��������� �����
    public CanvasGroup displayCanvasGroup; // Canvas Group ��� ����������� ��������
    public Sprite imageToShow; // ��������, ������� ����� ������������ ����� �����
    public float imageDisplayDuration = 3f; // ����� ����������� ��������
    public float fadeDuration = 1f; // ����� ��� �������� ���������
    public GameObject VideoCube;
    private int currentVideoIndex = 0; // ������� ������ �����
    private bool isSkipping = false;  // ���� �������� �����
    private bool isImageDisplaying = false; // ���� ����������� �����������

    void Start()
    {
        videoPlayers = GetComponents<VideoPlayer>();
        //Screen.SetResolution(1280, 960, false);
        //Screen.SetResolution(640, 480, false);

        if (videoPlayers.Length < 2)
        {
            Debug.LogError("�� ������� ������ ���� ��� ������� ��� ���������� VideoPlayer!");
            return;
        }

        if (displayCanvasGroup != null)
        {
            displayCanvasGroup.alpha = 0f;
        }
        // ��������� ������ �����
        currentVideoIndex = 0;
        videoPlayers[currentVideoIndex].loopPointReached += OnVideoEnd;
        videoPlayers[currentVideoIndex].Play();
    }

    void Update()
    {
        // ���������, ���� ������������ ����� ������� ��������
        if (!isImageDisplaying && (Input.GetButtonDown("Jump") || Input.GetButtonDown("Attack"))) // Space ��� ��������
        {
            SkipCurrentVideo();
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // ���� ����� �����������, ��������� � ����������
        PlayNextVideoOrImage();
    }

    void SkipCurrentVideo()
    {
        if (isSkipping) return; // �������� ������������� ������
        isSkipping = true;

        // ������������� ��������� ������� �����
        videoPlayers[currentVideoIndex].Stop();
        PlayNextVideoOrImage();
    }

    void PlayNextVideoOrImage()
    {
        isSkipping = false;

        // ���� ������� ������ ������������� ���������� �����, ���������� ��������
        if (currentVideoIndex < videoPlayers.Length - 1)
        {
            currentVideoIndex++;
            videoPlayers[currentVideoIndex].loopPointReached += OnVideoEnd;
            videoPlayers[currentVideoIndex].Play();
        }
        else
        {
            VideoCube.SetActive(false);
            ShowImage();
        }
    }

    public void ShowImage()
    {
        isImageDisplaying = true; // ������������� ����, ��� ����������� ������������
        if (displayCanvasGroup != null)
        {
            displayCanvasGroup.GetComponent<Image>().sprite = imageToShow;
            StartCoroutine(FadeInAndOutTransition());
        }
        else
        {
            TransitionToNextScene();
        }
    }

    System.Collections.IEnumerator FadeInAndOutTransition()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            displayCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        yield return new WaitForSeconds(imageDisplayDuration);

        elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            displayCanvasGroup.alpha = Mathf.Clamp01(1f - (elapsedTime / fadeDuration));
            yield return null;
        }

        isImageDisplaying = false; // ���������� ���� ����� ���������� ����������� �����������
        TransitionToNextScene();
    }

    void TransitionToNextScene()
    {
        //Screen.SetResolution(1024, 880, false);
        //Screen.SetResolution(512, 440, false);
        SceneManager.LoadScene(nextSceneName);
    }
}
