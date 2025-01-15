using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoDeathController : MonoBehaviour
{
    private VideoPlayer Death;
    // Update is called once per frame
    void Start()
    {
        Death = GetComponent<VideoPlayer>();
        Death.loopPointReached += OnDeathVideoEnd;
        Death.Play();
    }

    void Update()
    {
        // Проверяем, если пользователь нажал клавишу пропуска
        if ((Input.GetButtonDown("Jump") || Input.GetButtonDown("Attack"))) // Space для пропуска
        {
            SceneManager.LoadScene("VideoScene");
        }
    }
    void OnDeathVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene("VideoScene");
    }
}
