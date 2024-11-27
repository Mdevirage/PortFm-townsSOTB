using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject backgroundObject; // ������ ����������
    public Cinemachine.CinemachineVirtualCamera gameplayCamera; // �������� ������
    public Cinemachine.CinemachineVirtualCamera ladderCamera; // ������ ��������

    public void SwitchToLadderCamera()
    {
        // �������� ���������� � ������ ��������
        backgroundObject.transform.SetParent(ladderCamera.transform);

        // ������������ �����
        gameplayCamera.Priority = 0;
        ladderCamera.Priority = 10;
    }

    public void SwitchToGameplayCamera()
    {
        // �������� ���������� � �������� ������
        backgroundObject.transform.SetParent(gameplayCamera.transform);

        // ������������ �����
        ladderCamera.Priority = 0;
        gameplayCamera.Priority = 10;
    }
}
