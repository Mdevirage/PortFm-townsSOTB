using UnityEngine;

public class BossActivationByCamera : MonoBehaviour
{
    [Header("Ссылки")]
    public Camera mainCamera;           // Основная камера
    public BossTree bossTree;           // Ссылка на BossTree
    public BossMovement bossMovement;   // Ссылка на BossMovement

    [Header("Настройки видимости")]
    public float bufferX = 0.1f;        // Дополнительный буфер по оси X
    public float bufferY = 0.0f;        // Дополнительный буфер по оси Y

    private bool isActivated = false;    // Флаг, указывающий, был ли босс уже активирован
    public GameObject WallObject;
    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("BossActivationByCamera: Основная камера не найдена.");
                return;
            }
        }

        if (bossTree == null)
        {
            bossTree = GetComponentInParent<BossTree>();
            if (bossTree == null)
            {
                Debug.LogError("BossActivationByCamera: Компонент BossTree не найден в родительских объектах.");
                return;
            }
        }

        if (bossMovement == null)
        {
            bossMovement = GetComponentInParent<BossMovement>();
            if (bossMovement == null)
            {
                Debug.LogError("BossActivationByCamera: Компонент BossMovement не найден в родительских объектах.");
                return;
            }
        }

        // Изначально деактивируем компоненты босса
        bossTree.enabled = false;
        bossMovement.enabled = false;
    }

    void Update()
    {
        if (isActivated)
            return;

        // Получаем мировые координаты босса относительно камеры
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);

        // Проверяем, находится ли босс в пределах видимости камеры с учетом буфера
        bool isInCameraView = (viewportPos.x >= -bufferX && viewportPos.x <= 1 + bufferX
                                && viewportPos.y >= -bufferY && viewportPos.y <= 1 + bufferY
                                && viewportPos.z > 0f);

        if (isInCameraView)
        {
            isActivated = true;
            WallObject.SetActive(true);
            ActivateBoss();
        }
    }

    void ActivateBoss()
    {
        Debug.Log("BossActivationByCamera: Босс впервые увиден. Активация...");

        // Включаем компоненты босса
        if (bossTree != null)
            bossTree.enabled = true;

        if (bossMovement != null)
            bossMovement.enabled = true;

        // Дополнительные действия при активации, если необходимо
    }
}
