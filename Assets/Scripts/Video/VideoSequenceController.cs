using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VideoSequenceController : MonoBehaviour
{
    private VideoPlayer[] videoPlayers; // Массив для хранения VideoPlayer
    public string nextSceneName; // Имя начальной сцены
    public CanvasGroup displayCanvasGroup; // Canvas Group для отображения картинки
    public Sprite imageToShow; // Картинка, которая будет отображаться после видео
    public float imageDisplayDuration = 3f; // Время отображения картинки
    public float fadeDuration = 1f; // Время для плавного появления
    public GameObject VideoCube;
    private int currentVideoIndex = 0; // Текущий индекс видео
    private bool isSkipping = false;  // Флаг пропуска видео
    private bool isImageDisplaying = false; // Флаг отображения изображения

    void Start()
    {
        videoPlayers = GetComponents<VideoPlayer>();

        if (videoPlayers.Length < 2)
        {
            Debug.LogError("На объекте должно быть как минимум два компонента VideoPlayer!");
            return;
        }
        string videoPath1;
        string videoPath2;
        // Для WebGL используем относительный путь
        videoPath1 = System.IO.Path.Combine(Application.streamingAssetsPath, $"Intro.mp4");
        // Для других платформ путь из StreamingAssets
        videoPath2 = System.IO.Path.Combine(Application.streamingAssetsPath, $"tree.mp4");
        videoPlayers[0].url = videoPath1;
        videoPlayers[1].url = videoPath2;

        if (displayCanvasGroup != null)
        {
            displayCanvasGroup.alpha = 0f;
        }
        // Запускаем первое видео
        currentVideoIndex = 0;
        videoPlayers[currentVideoIndex].loopPointReached += OnVideoEnd;
        videoPlayers[currentVideoIndex].Play();
    }

    void Update()
    {
        // Проверяем, если пользователь нажал клавишу пропуска
        if (!isImageDisplaying && (Input.GetButtonDown("Jump") || Input.GetButtonDown("Attack"))) // Space для пропуска
        {
            SkipCurrentVideo();
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // Если видео завершилось, переходим к следующему
        PlayNextVideoOrImage();
    }

    void SkipCurrentVideo()
    {
        if (isSkipping) return; // Избегаем многократного вызова
        isSkipping = true;

        // Принудительно завершаем текущее видео
        videoPlayers[currentVideoIndex].Stop();
        PlayNextVideoOrImage();
    }

    void PlayNextVideoOrImage()
    {
        isSkipping = false;

        // Если текущий индекс соответствует последнему видео, показываем картинку
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
        isImageDisplaying = true; // Устанавливаем флаг, что изображение отображается
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

        isImageDisplaying = false; // Сбрасываем флаг после завершения отображения изображения
        TransitionToNextScene();
    }

    void TransitionToNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
