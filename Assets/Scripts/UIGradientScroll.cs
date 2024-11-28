using UnityEngine;
using UnityEngine.UI;

public class UIGradientScroll : MonoBehaviour
{
    public RawImage backgroundImage; // Ссылка на RawImage
    public float scrollSpeedY = 2f; // Пикселей в секунду
    private float timePerPixel; // Время на 1 пиксель
    private float timer = 0f; // Таймер для отслеживания времени

    void Start()
    {
        // Вычисляем, сколько времени требуется для сдвига на 1 пиксель
        timePerPixel = 1f / scrollSpeedY;
    }

    void Update()
    {
        // Увеличиваем таймер
        timer += Time.deltaTime;

        // Если прошло достаточно времени, сдвигаем фон
        if (timer >= timePerPixel)
        {
            // Обновляем UV-координаты
            float uvOffsetY = 2f / 56f; // Один пиксель в UV
            backgroundImage.uvRect = new Rect(backgroundImage.uvRect.x, backgroundImage.uvRect.y - uvOffsetY, backgroundImage.uvRect.width, backgroundImage.uvRect.height);

            // Сбрасываем таймер
            timer -= timePerPixel;
        }
    }
}
