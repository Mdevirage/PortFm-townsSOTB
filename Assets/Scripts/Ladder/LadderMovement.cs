using Cinemachine;
using UnityEngine;
using System.Collections;

public class LadderMovement : MonoBehaviour
{
    public float climbSpeed = 3.0f;
    public bool isClimbing = false;
    private bool isExitingClimb = false; // ���� ��� ���������� �������� ��� �������� ������
    private bool CameraM = false;

    private Rigidbody2D body;
    private Animator anim;

    public bool isTopDetectorActive = false;
    public bool isBottomDetectorActive = false;
    public bool isOverlapLadderActive = false;

    public CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer framingTransposer;
    public GameObject tilemapToDisable;
    private int playerLayer;
    private int climbingPlayerLayer;
    private int groundLayer;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        playerLayer = LayerMask.NameToLayer("Player");
        climbingPlayerLayer = LayerMask.NameToLayer("ClimbingPlayer");
        groundLayer = LayerMask.NameToLayer("groundLayer");
    }

    void Update()
    {
        if (isClimbing)
        {
            if (isExitingClimb)
                return; // ��������� ���������� ��� ������

            if (CameraM)
            {
                framingTransposer.m_ScreenY = 0.806f;
                framingTransposer.m_SoftZoneHeight = 0f;

            }

            float verticalInput = Input.GetAxis("Vertical");

            if (isClimbing)
            {

                // ������������� �������� ClimbSpeed ��� ���������� ���������
                anim.SetFloat("ClimbSpeed", verticalInput);

                // ��������� �������� ����������� ���������
                body.velocity = new Vector2(0, verticalInput * climbSpeed);

                if (verticalInput == 0)
                    body.velocity = Vector2.zero; // ��������� ��������� ��� ���������� �����
            }

            if ((!isTopDetectorActive && isBottomDetectorActive && Input.GetKey(KeyCode.UpArrow))
                || (isTopDetectorActive && !isBottomDetectorActive && Input.GetKey(KeyCode.DownArrow))
                || isOverlapLadderActive && (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow)))
            {
                StopClimbing();
            }
        }
        else
        {
            if (body.velocity.y == 0 && isTopDetectorActive && (Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow)) && Input.GetAxis("Vertical") != 0)
            {
                StartClimbing(true); // �������� ������
            }
            else if (body.velocity.y == 0 && isBottomDetectorActive && (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.UpArrow)) && Input.GetAxis("Vertical") != 0)
            {
                StartClimbing(false); // �������� �����
            }
        }
    }
    float horizontalposition;
    float direction;
    public void StartClimbing(bool isClimbingUp)
    {
        isClimbing = true;
        isExitingClimb = true; // ������� ���� ������
        gameObject.layer = climbingPlayerLayer;

        // ���������� �������� ����� ClimbingPlayer � Ground
        Physics2D.IgnoreLayerCollision(climbingPlayerLayer, groundLayer, true);


        body.gravityScale = 0;
        body.velocity = Vector2.zero;

        // ������� ����� ��������� �������� � ���������� ����������� �������
        BoxCollider2D closestLadder = FindClosestLadder();
        Vector2 ladderCenter = closestLadder.bounds.center;
        // ��������� �������� ������� ��� ������
        if (isClimbingUp)
        {
            if (closestLadder != null)
            {
                horizontalposition = ladderCenter.x;
                anim.SetTrigger("StartClimbUp"); // �������� ��� �������
                transform.position = new Vector2(ladderCenter.x, transform.position.y);
            }
        }
        else
        {
            if (closestLadder != null)
            {

                horizontalposition = ladderCenter.x;
                direction = transform.position.x - ladderCenter.x; // ������������� - �������� ������, ������������� - �����
                if (direction > 0)
                {
                    // �������� �������� ������, �������� �������� ������ ������
                    anim.SetTrigger("StartClimbDownRight");
                }
                else
                {
                    // �������� �������� �����, �������� �������� ������ �����
                    anim.SetTrigger("StartClimbDownLeft");
                }

                // ����������� ��������� �� ������ �������� (������������ ��� ����� �������)
                transform.position = new Vector2(ladderCenter.x, transform.position.y);
            }
        }
        anim.SetBool("IsClimbing", true);
    }

    public void StopClimbing()
    {
        isExitingClimb = true;// ���������� ���� ������
        Debug.Log("isClimbing False");
        gameObject.layer = playerLayer;

        // ��������������� �������� ����� ClimbingPlayer � Ground
        Physics2D.IgnoreLayerCollision(climbingPlayerLayer, groundLayer, false);

        body.gravityScale = 1;

        // ���������� ����������� ������
        if (isTopDetectorActive)
        {
            anim.SetTrigger("ExitClimbUp");
        }
        else if (isBottomDetectorActive)
        {
            anim.SetTrigger("ExitClimbDown");
        }

        anim.SetBool("IsClimbing", false);
    }

    public void OnStartClimbDownAnimationComplete()
    {
        CameraM = true;
        transform.position = new Vector2(horizontalposition, transform.position.y-1.8f);
    }
    public void OnStartClimbUpAnimationComplete()
    {
        CameraM = true;
        transform.position = new Vector2(horizontalposition, transform.position.y);
    }

    public void OnExitClimbComplete()
    {
        // ���������� �� ���������� �������� ������ (����� ������� ��������)
        isExitingClimb = false;
    }
    public void CamMove()
    {
        // ���������� �� ���������� �������� ������ (����� ������� ��������)
        CameraM = true;
    }
    public void OnExitClimbAnimationComplete()
    {
        // ���� ����� ���������� ��������� ����� � ����������
        isClimbing = false;
        isExitingClimb = false;
        CameraM = false;  // ������� ���� �������� ������, ���� ���������
        framingTransposer.m_ScreenY = 0.5682f;
        framingTransposer.m_SoftZoneHeight = 0.5f;
    }
    // ����� ��� ������ ��������� ��������
    // ����� ��� ������ ��������� ��������
    private BoxCollider2D FindClosestLadder()
    {
        // ������� �������������� ������ (������ � ������)
        Vector2 boxSize = new Vector2(1.0f, 3.0f);

        // �������� �������������� ������������ ������� ���������
        Vector2 boxOffset = new Vector2(0f, -1.75f);

        // ������� ������ ��������������
        Vector2 boxCenter = (Vector2)transform.position + boxOffset;

        // ����� ��� ���������� � ������� ��������������
        Collider2D[] colliders = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f);
        BoxCollider2D closestLadder = null;
        float closestDistance = Mathf.Infinity;

        foreach (var collider in colliders)
        {
            // ���������, �������� �� ������ ���������
            if (collider.CompareTag("Ladder") && collider is BoxCollider2D boxCollider)
            {
                float distance = Vector2.Distance(transform.position, boxCollider.bounds.center);
                if (distance < closestDistance)
                {
                    closestLadder = boxCollider;
                    closestDistance = distance;
                }
            }
        }

        return closestLadder;
    }


    public void DisableTilemap()
    {
        if (tilemapToDisable != null)
        {
            tilemapToDisable.SetActive(false);
            Debug.Log($"Tilemap {tilemapToDisable.name} ��������.");
        }
        else
        {
            Debug.LogWarning("Tilemap ��� ���������� �� ��������.");
        }
    }

    public void EnableTilemap()
    {
        if (tilemapToDisable != null)
        {
            tilemapToDisable.SetActive(true);
            Debug.Log($"Tilemap {tilemapToDisable.name} �������.");
        }
        else
        {
            Debug.LogWarning("Tilemap ��� ��������� �� ��������.");
        }
    }

    public void ClimbDownMovement(float offset)
    {
        transform.position = new Vector2(horizontalposition, transform.position.y + offset);
    }
}