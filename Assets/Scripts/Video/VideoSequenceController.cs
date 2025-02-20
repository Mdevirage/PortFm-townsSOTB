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

        if (videoPlayers.Length < 2)
        {
            Debug.LogError("�� ������� ������ ���� ��� ������� ��� ���������� VideoPlayer!");
            return;
        }
        string videoPath1;
        string videoPath2;
        // ��� WebGL ���������� ������������� ����
        videoPath1 = System.IO.Path.Combine(Application.streamingAssetsPath, $"Intro.mp4");
        // ��� ������ �������� ���� �� StreamingAssets
        videoPath2 = System.IO.Path.Combine(Application.streamingAssetsPath, $"tree.mp4");
        videoPlayers[0].url = videoPath1;
        videoPlayers[1].url = videoPath2;

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
        SceneManager.LoadScene(nextSceneName);
    }
}
