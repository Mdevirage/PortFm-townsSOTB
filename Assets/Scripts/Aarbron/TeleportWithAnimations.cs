using Cinemachine;
using UnityEngine;

public class TeleportWithAnimations : MonoBehaviour
{
    [Header("Animator и триггеры")]
    private Animator playerAnimator;

    [Header("Ссылка на точку выхода")]
    public Transform teleportOut; // Координаты, куда телепортируемся

    private PlatformerPlayer playerController;

    [Header("Другие настройки (при желанию)")]
    public bool isTeleporting = false;
    // Можно хранить состояние, чтобы заблокировать управление

    [Header("Cinemachine Settings")]
    public CinemachineVirtualCamera virtualCamera; // Ссылка на Virtual Camera
    private CinemachineFramingTransposer framingTransposer;

    private Vector3 preTeleportCameraPosition; // Позиция камеры до телепорта
    private Vector3 teleportDelta; // Разница позиций персонажа

    private void Start()
    {
        playerController = GetComponent<PlatformerPlayer>();
        playerAnimator = GetComponent<Animator>();
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    /// <summary>
    /// Запускает последовательность телепортации: проигрывает анимацию исчезновения.
    /// </summary>
    public void StartTeleportSequence()
    {
        if (!isTeleporting)
        {
            isTeleporting = true;

            // Блокируем управление персонажем
            playerController.isMovementLocked = true;

            // Запускаем анимацию исчезновения
            playerAnimator.SetTrigger("TeleportTrigger");
        }
    }

    /// <summary>
    /// Метод, вызываемый из Animation Event в конце анимации исчезновения.
    /// Выполняет телепорт и запускает анимацию появления.
    /// </summary>
    public void OnFadeOutComplete()
    {
        // Сохраняем текущее смещение камеры относительно персонажа
        preTeleportCameraPosition = virtualCamera.transform.position;
        teleportDelta = teleportOut.position - transform.position;

        // Информируем Cinemachine о телепорте
        virtualCamera.OnTargetObjectWarped(transform, teleportDelta);

        // Телепортируем персонажа
        transform.position = teleportOut.position;

        // Запускаем анимацию появления
        playerAnimator.SetTrigger("TeleportTriggerReverse");
    }

    /// <summary>
    /// Метод, вызываемый из Animation Event в конце анимации появления.
    /// Завершает телепортацию и разблокирует управление.
    /// </summary>
    public void OnFadeInComplete()
    {
        // Завершаем телепортацию
        isTeleporting = false;

        // Разблокируем управление персонажем
        playerController.isMovementLocked = false;
    }
}
