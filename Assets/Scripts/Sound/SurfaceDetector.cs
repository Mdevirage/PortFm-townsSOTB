using UnityEngine;

public class SurfaceDetector : MonoBehaviour
{
    public FootstepSound footstepSound; // ������ �� ������ FootstepSound
    public LandingSound landingSound;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder")) return;
        if (other.tag != "Ladder")
        { // �������� FootstepSound � �����������
            footstepSound.UpdateSurfaceTag(other.tag);
            landingSound.UpdateSurfaceTag(other.tag);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder")) return;
        if (other.tag != "Ladder")
        {
            // ���� �������� �������� �������, ���������� �� ����������� �����
            //Debug.Log("Reset");
            footstepSound.ResetSurfaceTag(other.tag);
            landingSound.ResetSurfaceTag(other.tag);
        }
    }
}
