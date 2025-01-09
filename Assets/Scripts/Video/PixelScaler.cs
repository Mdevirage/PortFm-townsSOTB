using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PixelScaler : MonoBehaviour
{
    public CanvasScaler canvasScaler; // ������ �� Canvas Scaler
    public int referenceWidth = 640;  // ������� ������
    public int referenceHeight = 480; // ������� ������

    void Start()
    {
        // �������� ������� ������ ������
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // ������������ ������� ������� ��� ������ � ������
        float scaleWidth = Mathf.Floor(screenWidth / referenceWidth);
        float scaleHeight = Mathf.Floor(screenHeight / referenceHeight);

        // ���� ����������� �������, ����� UI �� ������� �� ������� ������
        float finalScale = Mathf.Min(scaleWidth, scaleHeight);

        // ��������� ���������������
        canvasScaler.scaleFactor = finalScale; // ������������ Canvas
    }
}
