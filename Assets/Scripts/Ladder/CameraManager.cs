using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject backgroundObject; // Объект параллакса
    public Cinemachine.CinemachineVirtualCamera gameplayCamera; // Основная камера
    public Cinemachine.CinemachineVirtualCamera ladderCamera; // Камера лестницы

    public void SwitchToLadderCamera()
    {
        // Передача параллакса к камере лестницы
        backgroundObject.transform.SetParent(ladderCamera.transform);

        // Переключение камер
        gameplayCamera.Priority = 0;
        ladderCamera.Priority = 10;
    }

    public void SwitchToGameplayCamera()
    {
        // Передача параллакса к основной камере
        backgroundObject.transform.SetParent(gameplayCamera.transform);

        // Переключение камер
        ladderCamera.Priority = 0;
        gameplayCamera.Priority = 10;
    }
}
