using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoGroundDetect : MonoBehaviour
{
    public LadderMovement playerLadderMovement;

    void Start()
    {
        playerLadderMovement = GetComponentInParent<LadderMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("NogroundDetect"))
        {
            //Debug.Log("Внизу");
            playerLadderMovement.isGroundActive = true; // Активируем флаг для движения вниз
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("NogroundDetect"))
        {
            //Debug.Log("Выход из нижней детектора");
            playerLadderMovement.isGroundActive = false; // Деактивируем флаг для движения вниз
        }
    }
}
