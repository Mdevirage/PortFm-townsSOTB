using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform cameraTransform;  // ������ �� ������
    public float parallaxEffectMultiplier = 0.5f;  // ����������� ���������� �� �����������

    private Transform[] layers;  // ������ ����� ��� ����������
    private float viewZone = 5f;  // �������, �� ������� ��� ����� ������
    private int leftIndex;  // ������ ������ �����������
    private int rightIndex;  // ������ ������� �����������
    private float backgroundSize;  // ������ ���� �� �����������
    private Vector3 lastCameraPosition;  // ��������� ������� ������

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
        // �������� ���� �� �����������
        // ��������� �������� ����������
        float deltaX = cameraTransform.position.x - lastCameraPosition.x;
        Vector3 newPosition = transform.position - new Vector3(deltaX * parallaxEffectMultiplier, 0, 0);

        // ����������� ����� ������� � ���������� �����
        newPosition.x = Mathf.Round(newPosition.x * 32) / 32; // ����������� � ����� 32 ������� �� ����
        transform.position = newPosition;

        // ��������� ������� ������
        lastCameraPosition = cameraTransform.position;

        // ���� ������ ������������� �� ������� ������� �����������, �������� ����� ����������� ������
        if (cameraTransform.position.x > (layers[rightIndex].position.x - viewZone))
        {
            ScrollRight();
        }

        // ���� ������ ������������� �� ������� ������ �����������, �������� ������ ����������� �����
        if (cameraTransform.position.x < (layers[leftIndex].position.x + viewZone))
        {
            ScrollLeft();
        }
    }

    private void ScrollLeft()
    {
        // �������� ������ ����������� �����
        layers[rightIndex].position = new Vector3(layers[leftIndex].position.x - backgroundSize, layers[rightIndex].position.y, layers[rightIndex].position.z);

        // ��������� �������
        leftIndex = rightIndex;
        rightIndex--;
        if (rightIndex < 0)
        {
            rightIndex = layers.Length - 1;
        }
    }

    private void ScrollRight()
    {
        // �������� ����� ����������� ������
        layers[leftIndex].position = new Vector3(layers[rightIndex].position.x + backgroundSize, layers[leftIndex].position.y, layers[leftIndex].position.z);

        // ��������� �������
        rightIndex = leftIndex;
        leftIndex++;
        if (leftIndex == layers.Length)
        {
            leftIndex = 0;
        }
    }
}
