using Cinemachine;
using UnityEngine;

public class TeleportWithAnimations : MonoBehaviour
{
    [Header("Animator � ��������")]
    private Animator playerAnimator;

    [Header("������ �� ����� ������")]
    public Transform teleportOut; // ����������, ���� ���������������

    private PlatformerPlayer playerController;

    [Header("������ ��������� (��� �������)")]
    public bool isTeleporting = false;
    // ����� ������� ���������, ����� ������������� ����������

    [Header("Cinemachine Settings")]
    public CinemachineVirtualCamera virtualCamera; // ������ �� Virtual Camera
    private CinemachineFramingTransposer framingTransposer;

    private Vector3 preTeleportCameraPosition; // ������� ������ �� ���������
    private Vector3 teleportDelta; // ������� ������� ���������

    private void Start()
    {
        playerController = GetComponent<PlatformerPlayer>();
        playerAnimator = GetComponent<Animator>();
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    /// <summary>
    /// ��������� ������������������ ������������: ����������� �������� ������������.
    /// </summary>
    public void StartTeleportSequence()
    {
        if (!isTeleporting)
        {
            isTeleporting = true;

            // ��������� ���������� ����������
            playerController.isMovementLocked = true;

            // ��������� �������� ������������
            playerAnimator.SetTrigger("TeleportTrigger");
        }
    }

    /// <summary>
    /// �����, ���������� �� Animation Event � ����� �������� ������������.
    /// ��������� �������� � ��������� �������� ���������.
    /// </summary>
    public void OnFadeOutComplete()
    {
        // ��������� ������� �������� ������ ������������ ���������
        preTeleportCameraPosition = virtualCamera.transform.position;
        teleportDelta = teleportOut.position - transform.position;

        // ����������� Cinemachine � ���������
        virtualCamera.OnTargetObjectWarped(transform, teleportDelta);

        // ������������� ���������
        transform.position = teleportOut.position;

        // ��������� �������� ���������
        playerAnimator.SetTrigger("TeleportTriggerReverse");
    }

    /// <summary>
    /// �����, ���������� �� Animation Event � ����� �������� ���������.
    /// ��������� ������������ � ������������ ����������.
    /// </summary>
    public void OnFadeInComplete()
    {
        // ��������� ������������
        isTeleporting = false;

        // ������������ ���������� ����������
        playerController.isMovementLocked = false;
    }
}
