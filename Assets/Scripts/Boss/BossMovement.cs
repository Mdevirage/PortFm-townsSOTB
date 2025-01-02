using UnityEngine;
using System.Collections;

public class BossMovement : MonoBehaviour
{
    public Vector2 moveDirection = Vector2.left;        // ����������� �������� (��������, �����)
    public float initialMoveDistance = 5f;             // ��������� ���������� �������� �����
    public float moveSpeed = 2f;                       // �������� �������� �����
    public int moveIterations = 5;                     // ���������� �������� ��������
    public float moveIncrement = 2f;                   // ���������� ���������� �� ������ ��������

    private int currentIteration = 0;                  // ������� ��������
    private Vector3 startPosition;                     // ��������� ������� �����
    private float targetDistance;                      // ������� ��������� ��� ������� ��������
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
            // ������������� ��������, ���� ���� �����
            StopAllCoroutines();
            body.velocity = Vector2.zero;
            // �������������� �������� ��� ������, ���� ����������
            return;
        }
    }

    IEnumerator MoveBoss()
    {
        while (currentIteration < moveIterations)
        {
            // 1. ����������� �����
            Vector3 forwardTarget = startPosition + (Vector3)(moveDirection.normalized * targetDistance);
            yield return StartCoroutine(MoveToPosition(forwardTarget));

            currentIteration++;

            // ���������, �� ��������� �� ��� ��������
            if (currentIteration < moveIterations)
            {
                // 2. ����������� ����� � ��������� �������
                yield return StartCoroutine(MoveToPosition(startPosition));

                // 3. ����������� ��������� ��� ��������� ��������
                targetDistance = initialMoveDistance + moveIncrement * currentIteration;
            }
            else
            {
                // ��������� ��������: �� ������������ �����
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
