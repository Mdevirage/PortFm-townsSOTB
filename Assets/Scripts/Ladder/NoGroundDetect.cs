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
            //Debug.Log("�����");
            playerLadderMovement.isGroundActive = true; // ���������� ���� ��� �������� ����
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("NogroundDetect"))
        {
            //Debug.Log("����� �� ������ ���������");
            playerLadderMovement.isGroundActive = false; // ������������ ���� ��� �������� ����
        }
    }
}
