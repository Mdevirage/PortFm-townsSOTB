using UnityEngine;

public class BossActivationByCamera : MonoBehaviour
{
    [Header("������")]
    public Camera mainCamera;           // �������� ������
    public BossTree bossTree;           // ������ �� BossTree
    public BossMovement bossMovement;   // ������ �� BossMovement

    [Header("��������� ���������")]
    public float bufferX = 0.1f;        // �������������� ����� �� ��� X
    public float bufferY = 0.0f;        // �������������� ����� �� ��� Y

    private bool isActivated = false;    // ����, �����������, ��� �� ���� ��� �����������
    public GameObject WallObject;
    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("BossActivationByCamera: �������� ������ �� �������.");
                return;
            }
        }

        if (bossTree == null)
        {
            bossTree = GetComponentInParent<BossTree>();
            if (bossTree == null)
            {
                Debug.LogError("BossActivationByCamera: ��������� BossTree �� ������ � ������������ ��������.");
                return;
            }
        }

        if (bossMovement == null)
        {
            bossMovement = GetComponentInParent<BossMovement>();
            if (bossMovement == null)
            {
                Debug.LogError("BossActivationByCamera: ��������� BossMovement �� ������ � ������������ ��������.");
                return;
            }
        }

        // ���������� ������������ ���������� �����
        bossTree.enabled = false;
        bossMovement.enabled = false;
    }

    void Update()
    {
        if (isActivated)
            return;

        // �������� ������� ���������� ����� ������������ ������
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);

        // ���������, ��������� �� ���� � �������� ��������� ������ � ������ ������
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
        Debug.Log("BossActivationByCamera: ���� ������� ������. ���������...");

        // �������� ���������� �����
        if (bossTree != null)
            bossTree.enabled = true;

        if (bossMovement != null)
            bossMovement.enabled = true;

        // �������������� �������� ��� ���������, ���� ����������
    }
}
