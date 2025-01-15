using UnityEngine;
using System.Collections.Generic;

public class SurfaceDetector : MonoBehaviour
{
    public FootstepSound footstepSound; // ������ �� ������ FootstepSound
    public LandingSound landingSound;

    // ������ ������� ������������
    private List<Collider2D> currentSurfaces = new List<Collider2D>();

    // ����� ��� ����������� ���������� �����������
    private int GetSurfacePriority(string tag)
    {
        // ���������� ���������� � ����������� �� �����
        // ��������, "Wood" ����� ��������� 1, "Ground" � 0
        switch (tag)
        {
            case "Wood":
                return 1;
            case "Ground":
                return 0;
            // �������� ������ ���� � �� ���������� �� �������������
            default:
                return -1;
        }
    }

    // ����� ��� ���������� ������� �������� �����������
    private void UpdateActiveSurface()
    {
        if (currentSurfaces.Count == 0)
        {
            // ���� ��� ������������, ����� ���������� �����������
            footstepSound.ResetSurfaceTag("Ground");
            landingSound.ResetSurfaceTag("Ground");
            return;
        }

        // ����� ����������� � ��������� �����������
        Collider2D highestPrioritySurface = null;
        int highestPriority = int.MinValue;

        foreach (var surface in currentSurfaces)
        {
            int priority = GetSurfacePriority(surface.tag);
            if (priority > highestPriority)
            {
                highestPriority = priority;
                highestPrioritySurface = surface;
            }
        }

        if (highestPrioritySurface != null)
        {
            footstepSound.UpdateSurfaceTag(highestPrioritySurface.tag);
            landingSound.UpdateSurfaceTag(highestPrioritySurface.tag);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder")) return;
        if (other != null)
        {
            // ��������� ����������� � ������, ���� � ��� ���
            if (!currentSurfaces.Contains(other))
            {
                currentSurfaces.Add(other);
                UpdateActiveSurface();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder")) return;

        // ������� ����������� �� ������
        if (currentSurfaces.Contains(other))
        {
            currentSurfaces.Remove(other);
            UpdateActiveSurface();
        }
    }
}
