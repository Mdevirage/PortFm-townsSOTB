using UnityEngine;

public class SurfaceDetector : MonoBehaviour
{
    public FootstepSound footstepSound; // Ссылка на скрипт FootstepSound
    public LandingSound landingSound;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder")) return;
        if (other.tag != "Ladder")
        { // Сообщаем FootstepSound о поверхности
            footstepSound.UpdateSurfaceTag(other.tag);
            landingSound.UpdateSurfaceTag(other.tag);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder")) return;
        if (other.tag != "Ladder")
        {
            // Если персонаж покидает триггер, сбрасываем на стандартные звуки
            //Debug.Log("Reset");
            footstepSound.ResetSurfaceTag(other.tag);
            landingSound.ResetSurfaceTag(other.tag);
        }
    }
}
