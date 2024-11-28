using UnityEngine;
using UnityEngine.UI;

public class UIGradientScroll : MonoBehaviour
{
    public RawImage backgroundImage; // ������ �� RawImage
    public float scrollSpeedY = 2f; // �������� � �������
    private float timePerPixel; // ����� �� 1 �������
    private float timer = 0f; // ������ ��� ������������ �������

    void Start()
    {
        // ���������, ������� ������� ��������� ��� ������ �� 1 �������
        timePerPixel = 1f / scrollSpeedY;
    }

    void Update()
    {
        // ����������� ������
        timer += Time.deltaTime;

        // ���� ������ ���������� �������, �������� ���
        if (timer >= timePerPixel)
        {
            // ��������� UV-����������
            float uvOffsetY = 2f / 56f; // ���� ������� � UV
            backgroundImage.uvRect = new Rect(backgroundImage.uvRect.x, backgroundImage.uvRect.y - uvOffsetY, backgroundImage.uvRect.width, backgroundImage.uvRect.height);

            // ���������� ������
            timer -= timePerPixel;
        }
    }
}
