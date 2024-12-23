using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlapDetector : MonoBehaviour
{
    public LadderMovement playerLadderMovement;

    void Start()
    {
        playerLadderMovement = GetComponentInParent<LadderMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("OverLapLadder"))
        {
            //Debug.Log("���� � Over ��������");
            playerLadderMovement.isOverlapLadderActive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("OverLapLadder"))
        {
            //Debug.Log("����� �� Over ���������");
            playerLadderMovement.isOverlapLadderActive = false;
        }
    }
}
