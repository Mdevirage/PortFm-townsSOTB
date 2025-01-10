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
        //Screen.SetResolution(1280, 960, false);
        Screen.SetResolution(640, 480, false);
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
