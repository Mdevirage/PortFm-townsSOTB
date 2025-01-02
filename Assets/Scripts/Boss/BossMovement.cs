using UnityEngine;
using System.Collections;

public class BossMovement : MonoBehaviour
{
    public Vector2 moveDirection = Vector2.left;        // Направление движения (например, влево)
    public float initialMoveDistance = 5f;             // Начальное расстояние движения вперёд
    public float moveSpeed = 2f;                       // Скорость движения босса
    public int moveIterations = 5;                     // Количество итераций движения
    public float moveIncrement = 2f;                   // Увеличение расстояния на каждую итерацию

    private int currentIteration = 0;                  // Текущая итерация
    private Vector3 startPosition;                     // Начальная позиция босса
    private float targetDistance;                      // Целевая дистанция для текущей итерации
    public BossTree bossTree;
    private Rigidbody2D body;

    void Start()
    {
        startPosition = transform.position;
        targetDistance = initialMoveDistance;
        body = GetComponent<Rigidbody2D>();
        StartCoroutine(MoveBoss());
    }

    void Update()
    {
        if (bossTree != null && bossTree.isDead)
        {
            // Останавливаем движение, если босс мертв
            StopAllCoroutines();
            body.velocity = Vector2.zero;
            // Дополнительные действия при смерти, если необходимо
            return;
        }
    }

    IEnumerator MoveBoss()
    {
        while (currentIteration < moveIterations)
        {
            // 1. Перемещение вперёд
            Vector3 forwardTarget = startPosition + (Vector3)(moveDirection.normalized * targetDistance);
            yield return StartCoroutine(MoveToPosition(forwardTarget));

            currentIteration++;

            // Проверяем, не последняя ли это итерация
            if (currentIteration < moveIterations)
            {
                // 2. Перемещение назад к начальной позиции
                yield return StartCoroutine(MoveToPosition(startPosition));

                // 3. Увеличиваем дистанцию для следующей итерации
                targetDistance = initialMoveDistance + moveIncrement * currentIteration;
            }
            else
            {
                // Последняя итерация: не возвращаемся назад
                break;
            }
        }
    }

    IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            body.MovePosition(newPosition);
            yield return null;
        }

        body.MovePosition(targetPosition);
    }
}
