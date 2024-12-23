using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringCode : MonoBehaviour
{
    private AudioSource demonSound; // �������� �����
    private Camera mainCamera; // ������� ������

    void Start()
    {
        demonSound = GetComponent<AudioSource>();
        mainCamera = Camera.main; // �������� �������� ������
    }

    void Update()
    {
        CheckVisibility();
    }

    private void CheckVisibility()
    {
        // ����������� ������� ������� � ���������� ��������
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);

        // ���������, ��������� �� ������ � �������� ������� ������� ������
        bool isVisible = viewportPosition.z > 0 &&
                         viewportPosition.x > 0 && viewportPosition.x < 1 &&
                         viewportPosition.y > 0 && viewportPosition.y < 1;

        // ���������� ��� ��������� ���� �� ������ ���������
        if (isVisible && !demonSound.enabled)
        {
            demonSound.enabled = true;
        }
        else if (!isVisible && demonSound.enabled)
        {
            demonSound.enabled = false;
        }
    }
}
