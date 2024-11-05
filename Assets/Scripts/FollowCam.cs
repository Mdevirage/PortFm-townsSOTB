using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform target; // ÷ель, за которой следует камера (например, персонаж)
    public float smoothSpeed = 0.2f; // —корость сглаживани€ движени€ камеры
    public float fixedYPosition = 5f; // ‘иксированна€ высота камеры по оси Y
    public float verticalOffset = 0f; // —мещение камеры по вертикали относительно персонажа

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        // ѕозици€ цели с учетом смещени€ по горизонтали
        float targetX = target.position.x;

        // ≈сли персонаж ниже фиксированной высоты, камера будет следовать за ним по вертикали с учетом смещени€
        float targetY = transform.position.y;

        if (target.position.y < fixedYPosition)
        {
            targetY = target.position.y + verticalOffset;
        }
        else
        {
            targetY = fixedYPosition + verticalOffset;
        }

        // Ќова€ позици€ камеры - следуем за целью по X и по Y при падении
        Vector3 targetPosition = new Vector3(targetX, targetY, transform.position.z);

        // ѕлавное перемещение камеры с использованием Lerp
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }
}