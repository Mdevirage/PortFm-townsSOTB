using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demon : MonoBehaviour
{
    public GameObject firingObject;
    public AudioSource demonSound; // �������� �����
    public Transform player; // ������ �� ������
    public Sphere sphere;
    public void ShowObject()
    {
        firingObject.SetActive(true);
    }
    public void OffObject()
    {
        firingObject.SetActive(false);
    }
    private void OnBecameVisible()
    {
        demonSound.enabled = true; // ���������� ������
    }
    // ���� ����� ����������, ����� ������ ���������� ��������� �������
    private void OnBecameInvisible()
    {
        demonSound.enabled = false;
    }
    public void TriggerJump() 
    {
        sphere.StartJump();
    }
}
