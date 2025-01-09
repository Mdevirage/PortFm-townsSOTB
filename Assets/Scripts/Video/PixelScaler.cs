using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PixelScaler : MonoBehaviour
{
    public CanvasScaler canvasScaler; // Ссылка на Canvas Scaler
    public int referenceWidth = 640;  // Базовая ширина
    public int referenceHeight = 480; // Базовая высота

    void Start()
    {
        // Получаем текущий размер экрана
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Рассчитываем кратный масштаб для ширины и высоты
        float scaleWidth = Mathf.Floor(screenWidth / referenceWidth);
        float scaleHeight = Mathf.Floor(screenHeight / referenceHeight);

        // Берём минимальный масштаб, чтобы UI не выходил за пределы экрана
        float finalScale = Mathf.Min(scaleWidth, scaleHeight);

        // Применяем масштабирование
        canvasScaler.scaleFactor = finalScale; // Масштабируем Canvas
    }
}
