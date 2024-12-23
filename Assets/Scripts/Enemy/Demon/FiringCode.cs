using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringCode : MonoBehaviour
{
    private AudioSource demonSound; // Источник звука
    private Camera mainCamera; // Главная камера

    void Start()
    {
        demonSound = GetComponent<AudioSource>();
        mainCamera = Camera.main; // Получаем основную камеру
    }

    void Update()
    {
        CheckVisibility();
    }

    private void CheckVisibility()
    {
        // Преобразуем позицию объекта в координаты вьюпорта
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);

        // Проверяем, находится ли объект в пределах видимой области камеры
        bool isVisible = viewportPosition.z > 0 &&
                         viewportPosition.x > 0 && viewportPosition.x < 1 &&
                         viewportPosition.y > 0 && viewportPosition.y < 1;

        // Активируем или отключаем звук на основе видимости
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
