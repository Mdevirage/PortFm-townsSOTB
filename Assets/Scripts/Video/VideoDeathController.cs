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
        string videoPath;
        // ��� WebGL ���������� ������������� ����
        videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, $"Death.mp4");
        Death.url = videoPath;

        Death.loopPointReached += OnDeathVideoEnd;
        Death.Play();
    }

    void Update()
    {
        // ���������, ���� ������������ ����� ������� ��������
        if ((Input.GetButtonDown("Jump") || Input.GetButtonDown("Attack"))) // Space ��� ��������
        {
            SceneManager.LoadScene("VideoScene");
        }
    }
    void OnDeathVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene("VideoScene");
    }
}
