using UnityEngine;
using System.Collections.Generic;

public class SurfaceDetector : MonoBehaviour
{
    public FootstepSound footstepSound; // Ссылка на скрипт FootstepSound
    public LandingSound landingSound;

    // Список текущих поверхностей
    private List<Collider2D> currentSurfaces = new List<Collider2D>();

    // Метод для определения приоритета поверхности
    private int GetSurfacePriority(string tag)
    {
        // Определите приоритеты в зависимости от тегов
        // Например, "Wood" имеет приоритет 1, "Ground" — 0
        switch (tag)
        {
            case "Wood":
                return 1;
            case "Ground":
                return 0;
            // Добавьте другие теги и их приоритеты по необходимости
            default:
                return -1;
        }
    }

    // Метод для обновления текущей активной поверхности
    private void UpdateActiveSurface()
    {
        if (currentSurfaces.Count == 0)
        {
            // Если нет поверхностей, можно установить стандартную
            footstepSound.ResetSurfaceTag("Ground");
            landingSound.ResetSurfaceTag("Ground");
            return;
        }

        // Найти поверхность с наивысшим приоритетом
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
            // Добавляем поверхность в список, если её ещё нет
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

        // Удаляем поверхность из списка
        if (currentSurfaces.Contains(other))
        {
            currentSurfaces.Remove(other);
            UpdateActiveSurface();
        }
    }
}
