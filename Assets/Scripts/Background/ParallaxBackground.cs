using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform cameraTransform;  // Ссылка на камеру
    public float parallaxEffectMultiplier = 0.5f;  // Коэффициент параллакса по горизонтали

    private Transform[] layers;  // Массив слоев для параллакса
    private float viewZone = 5f;  // Граница, за которой фон будет смещен
    private int leftIndex;  // Индекс левого изображения
    private int rightIndex;  // Индекс правого изображения
    private float backgroundSize;  // Размер фона по горизонтали
    private Vector3 lastCameraPosition;  // Последняя позиция камеры

    void Start()
    {
        lastCameraPosition = cameraTransform.position;
        layers = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            layers[i] = transform.GetChild(i);
        }
        leftIndex = 0;
        rightIndex = layers.Length - 1;
        backgroundSize = layers[0].GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void LateUpdate()
    {
        // Движение фона по горизонтали
        // Вычисляем смещение параллакса
        float deltaX = cameraTransform.position.x - lastCameraPosition.x;
        Vector3 newPosition = transform.position - new Vector3(deltaX * parallaxEffectMultiplier, 0, 0);

        // Привязываем новую позицию к пиксельной сетке
        newPosition.x = Mathf.Round(newPosition.x * 32) / 32; // Привязываем к сетке 32 пикселя на юнит
        transform.position = newPosition;

        // Обновляем позицию камеры
        lastCameraPosition = cameraTransform.position;

        // Если камера переместилась за пределы правого изображения, сдвигаем левое изображение вправо
        if (cameraTransform.position.x > (layers[rightIndex].position.x - viewZone))
        {
            ScrollRight();
        }

        // Если камера переместилась за пределы левого изображения, сдвигаем правое изображение влево
        if (cameraTransform.position.x < (layers[leftIndex].position.x + viewZone))
        {
            ScrollLeft();
        }
    }

    private void ScrollLeft()
    {
        // Сдвигаем правое изображение влево
        layers[rightIndex].position = new Vector3(layers[leftIndex].position.x - backgroundSize, layers[rightIndex].position.y, layers[rightIndex].position.z);

        // Обновляем индексы
        leftIndex = rightIndex;
        rightIndex--;
        if (rightIndex < 0)
        {
            rightIndex = layers.Length - 1;
        }
    }

    private void ScrollRight()
    {
        // Сдвигаем левое изображение вправо
        layers[leftIndex].position = new Vector3(layers[rightIndex].position.x + backgroundSize, layers[leftIndex].position.y, layers[leftIndex].position.z);

        // Обновляем индексы
        rightIndex = leftIndex;
        leftIndex++;
        if (leftIndex == layers.Length)
        {
            leftIndex = 0;
        }
    }
}
