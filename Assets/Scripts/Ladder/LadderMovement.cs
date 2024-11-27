using Cinemachine;
using UnityEngine;
using System.Collections;
using Unity.Mathematics;
using TMPro;

public class LadderMovement : MonoBehaviour
{
    public float climbSpeed = 3.0f;
    public bool isClimbing = false;
    private bool isExitingClimb = false; // ���� ��� ���������� �������� ��� �������� ������

    private Rigidbody2D body;
    private Animator anim;

    public bool isTopDetectorActive = false;
    public bool isBottomDetectorActive = false;
    public bool isOverlapLadderActive = false;
    public bool isGroundActive = false;
    public CinemachineVirtualCamera virtualCamera;
    public GameObject tilemapToDisable;
    public GameObject SmoothObject;
    private int playerLayer;
    private int climbingPlayerLayer;
    private int groundLayer;
    private CinemachineFramingTransposer framingTransposer;
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        //cameraManager = FindObjectOfType<CameraManager>();
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


            float verticalInput = Input.GetAxis("Vertical");

            if (isClimbing)
            {

                // ������������� �������� ClimbSpeed ��� ���������� ���������
                anim.SetFloat("ClimbSpeed", verticalInput);

                // ��������� �������� ����������� ���������
                body.velocity = new Vector2(0, verticalInput * climbSpeed);

                if (math.abs(verticalInput) <= 0.5)
                {
                    body.velocity = Vector2.zero; // ��������� ��������� ��� ���������� �����
                    anim.SetFloat("ClimbSpeed",0);
                }
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
    float previousPositionX;
    public void StartClimbing(bool isClimbingUp)
    {
        virtualCamera.Follow = null;
        isClimbing = true;
        isExitingClimb = true; // ������� ���� ������
        gameObject.layer = climbingPlayerLayer;

        // ���������� �������� ����� ClimbingPlayer � Ground
        Physics2D.IgnoreLayerCollision(climbingPlayerLayer, groundLayer, true);

        BoxCollider2D closestLadder = FindClosestLadder();
        Vector2 ladderCenter = closestLadder.bounds.center;
        float ladderX = ladderCenter.x;
        body.gravityScale = 0;
        body.velocity = Vector2.zero;
        float characterDirection = Mathf.Sign(transform.localScale.x);
        float approachDirection = Mathf.Sign(transform.position.x - ladderX);
        previousPositionX = transform.position.x;
        horizontalposition = ladderCenter.x;
        direction = transform.position.x - ladderCenter.x;
        // ���� �������� ������� � ��������������� ������� �� ��������
        if (characterDirection == approachDirection)
        {
            StartTurnAnimation(approachDirection, isClimbingUp); // ��������� �������� ��������
            return; // ���� ���������� ��������, ����� ����������
        }
        // ������� ����� ��������� �������� � ���������� ����������� �������
        
        // ��������� �������� ������� ��� ������
        if (isClimbingUp)
        {
            if (closestLadder != null)
            {
                anim.SetTrigger("StartClimbUp"); // �������� ��� �������
                transform.position = new Vector2(ladderCenter.x, transform.position.y);
            }
        }
        else
        {
            if (closestLadder != null)
            { 
                Debug.Log($"Direction {direction} and {previousPositionX}");
                if (direction > 0)
                {
                    // �������� �������� ������, �������� �������� ������ ������
                    anim.SetTrigger("StartClimbDownRight");
                }
                if (direction < 0)
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
        if (isTopDetectorActive && isGroundActive && !isBottomDetectorActive)
        {
            Debug.Log("�������� ���������� - �������� �������� ������");
            ExitToFallingState();
            return;
        }
        // ���������� ����������� ������
        if (isTopDetectorActive)
        {
            anim.SetTrigger("ExitClimbUp");
            gameObject.layer = playerLayer;

            // ��������������� �������� ����� ClimbingPlayer � Ground
            Physics2D.IgnoreLayerCollision(climbingPlayerLayer, groundLayer, false);

            body.gravityScale = 1;

        }
        else if (isBottomDetectorActive)
        {
            if (math.sign(direction) > 0)
            {
                anim.SetTrigger("ExitClimbDownR");
                transform.position = new Vector2(transform.position.x, transform.position.y + 1.68f);
                gameObject.layer = playerLayer;

                // ��������������� �������� ����� ClimbingPlayer � Ground
                Physics2D.IgnoreLayerCollision(climbingPlayerLayer, groundLayer, false);

                body.gravityScale = 1;
            }
            else if (math.sign(direction) < 0)
            {
                anim.SetTrigger("ExitClimbDownL");
                transform.position = new Vector2(transform.position.x, transform.position.y + 1.68f);
                gameObject.layer = playerLayer;
                // ��������������� �������� ����� ClimbingPlayer � Ground
                Physics2D.IgnoreLayerCollision(climbingPlayerLayer, groundLayer, false);

                body.gravityScale = 1;
            }

        }

            anim.SetBool("IsClimbing", false);
    }

    public void OnStartClimbDownAnimationComplete()
    {
        transform.position = new Vector2(horizontalposition, transform.position.y - 1.7f);
        framingTransposer.m_ScreenY = 0.8155f;
        framingTransposer.m_SoftZoneHeight = 0.01f;
        SmoothObject.transform.SetParent(transform);
    }
    public void OnStartClimbUpAnimationComplete()
    {
        transform.position = new Vector2(horizontalposition, transform.position.y);
        framingTransposer.m_ScreenY = 0.8155f;
        framingTransposer.m_SoftZoneHeight = 0.01f;
        SmoothObject.transform.SetParent(transform);
    }

    public void OnExitClimbComplete()
    {
        // ���������� �� ���������� �������� ������ (����� ������� ��������)
        isExitingClimb = false;
    }
    
    public void OnExitClimbAnimationComplete()
    {
        // ���� ����� ���������� ��������� ����� � ����������
        transform.position = new Vector2(previousPositionX, transform.position.y);
        isClimbing = false;
        isExitingClimb = false;
        
        SmoothObject.transform.SetParent(null);
        framingTransposer.m_ScreenY = 0.5773f;
        framingTransposer.m_SoftZoneHeight = 0.5f;
        virtualCamera.Follow = transform;
    }
    
    public void OnExitClimbDownAnimationComplete()
    {
        // ���� ����� ���������� ��������� ����� � ����������
        isClimbing = false;
        isExitingClimb = false;
        transform.position = new Vector2(previousPositionX, transform.position.y);
        framingTransposer.m_ScreenY = 0.5773f;
        framingTransposer.m_SoftZoneHeight = 0.5f;
        SmoothObject.transform.SetParent(null);
        virtualCamera.Follow = transform;
    }
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
    
    private void ExitToFallingState()
    {
        isClimbing = false;

        anim.SetTrigger("IsJumping");

        // ���������� �������� � �������� ����������
        gameObject.layer = playerLayer;
        Physics2D.IgnoreLayerCollision(climbingPlayerLayer, groundLayer, false);
        body.gravityScale = 1;
        SmoothObject.transform.SetParent(null);
        framingTransposer.m_ScreenY = 0.5773f;
        framingTransposer.m_SoftZoneHeight = 0.5f;
        transform.position = new Vector3(previousPositionX,transform.position.y);
        virtualCamera.Follow = transform;
        Debug.Log("������� � ��������� �������");
    }
    public void CameraClimbDown()
    {
        SmoothObject.transform.position = new Vector2(previousPositionX, transform.position.y);
        virtualCamera.Follow = SmoothObject.transform;
        Vector3 targetPosition = new Vector3(previousPositionX, transform.position.y - 1.7f, transform.position.z);
        MoveToPosition(targetPosition, 0.8f);
    }
    public void CameraClimbUp()
    {
        SmoothObject.transform.position = new Vector2(previousPositionX, transform.position.y);
        virtualCamera.Follow = SmoothObject.transform;
    }
    public void CameraClimbDownRev()
    {
        framingTransposer.m_ScreenY = 0.76f;
        framingTransposer.m_SoftZoneHeight = 0.15f;
        float adjustedY = Mathf.Round(SmoothObject.transform.position.y + 1.9f);
        float offset = 0.083f;
        Vector3 targetPosition = new Vector3(previousPositionX, adjustedY-offset, transform.position.z);
        //Vector3 targetPosition = new Vector3(previousPositionX, SmoothObject.transform.position.y + 1.9f, transform.position.z);
        MoveToPosition(targetPosition, 0.8f);
        
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
    public void StartTurnAnimation(float approachDirection, bool isClimbingUp)
    {
        // ������� ��������� �� �����
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        // ������ �������� ��������, ���� ��� ����
        anim.SetTrigger("Turn");

        // ����� ���������� �������� ���������� �����
        StartCoroutine(WaitForTurnAndClimb(approachDirection,isClimbingUp));
    }

    private IEnumerator WaitForTurnAndClimb(float approachDirection, bool isClimbingUp)
    {
        yield return new WaitForSeconds(0.2f); // �������� ���������� �������� �������� (����������� �� ������������ ����� ��������)

        previousPositionX = transform.position.x;

        if (isClimbingUp)
        {
            // �������� �������
            anim.SetBool("IsClimbing", true);
            transform.position = new Vector2(horizontalposition, transform.position.y);
            anim.SetTrigger("StartClimbUp");
        }
        else
        {
            // �������� ������ � ����������� �� �����������
            if (approachDirection > 0)
            {
                anim.SetTrigger("StartClimbDownRight");
            }
            if (approachDirection < 0)
            {
                // �������� �������� �����, �������� �������� ������ �����
                anim.SetTrigger("StartClimbDownLeft");
            }

            anim.SetBool("IsClimbing", true);
            transform.position = new Vector2(horizontalposition, transform.position.y);
            Debug.Log($"ApproachDirection {approachDirection}");
        }
    }
    public void MoveToPosition(Vector3 targetPosition, float duration)
    {
        StartCoroutine(SmoothMove(SmoothObject.transform.position, targetPosition, duration));
    }

    private IEnumerator SmoothMove(Vector3 startPosition, Vector3 endPosition, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            SmoothObject.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SmoothObject.transform.position = endPosition; // ���������, ��� ������ ����� �� ������� �������
    }
}