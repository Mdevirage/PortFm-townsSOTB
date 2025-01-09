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
        //Screen.SetResolution(1280, 960, false);
        //Screen.SetResolution(640, 480, false);

        if (videoPlayers.Length < 2)
        {
            Debug.LogError("На объекте должно быть как минимум два компонента VideoPlayer!");
            return;
        }

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
        //Screen.SetResolution(1024, 880, false);
        //Screen.SetResolution(512, 440, false);
        SceneManager.LoadScene(nextSceneName);
    }
}
