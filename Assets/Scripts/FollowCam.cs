using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform target; // ����, �� ������� ������� ������ (��������, ��������)
    public float smoothSpeed = 0.2f; // �������� ����������� �������� ������
    public float fixedYPosition = 5f; // ������������� ������ ������ �� ��� Y
    public float verticalOffset = 0f; // �������� ������ �� ��������� ������������ ���������

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        // ������� ���� � ������ �������� �� �����������
        float targetX = target.position.x;

        // ���� �������� ���� ������������� ������, ������ ����� ��������� �� ��� �� ��������� � ������ ��������
        float targetY = transform.position.y;

        if (target.position.y < fixedYPosition)
        {
            targetY = target.position.y + verticalOffset;
        }
        else
        {
            targetY = fixedYPosition + verticalOffset;
        }

        // ����� ������� ������ - ������� �� ����� �� X � �� Y ��� �������
        Vector3 targetPosition = new Vector3(targetX, targetY, transform.position.z);

        // ������� ����������� ������ � �������������� Lerp
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }
}