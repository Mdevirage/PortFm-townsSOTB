using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demon : MonoBehaviour
{
    public GameObject firingObject;
    public Transform player; // —сылка на игрока
    public Sphere sphere;
    public void ShowObject()
    {
        firingObject.SetActive(true);
    }
    public void OffObject()
    {
        firingObject.SetActive(false);
    }
    public void TriggerJump()
    {
        sphere.StartJump();
    }
}