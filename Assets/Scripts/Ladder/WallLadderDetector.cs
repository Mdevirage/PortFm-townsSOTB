using UnityEngine;

public class WallLadderDetector : MonoBehaviour
{
    private LadderMovement playerLadderMovement;
    private PlatformerPlayer player;
    public LayerMask wallLayer; // ���� ��� ����
    public Vector2 boxSize = new Vector2(1.5f, 3.825f); // ������ ������� ��������
    public Vector2 boxOffset = new Vector2(0f, -0.275f); // �������� ������� ��������

    private void Start()
    {
        player = GetComponentInParent<PlatformerPlayer>();
        playerLadderMovement = GetComponentInParent<LadderMovement>();
    }
    void Update()
    {
        // ������� ������ ��������
        Vector2 checkPosition = (Vector2)transform.position + boxOffset;

        // ��������� ������� ���� � �������
        Collider2D wallCollider = Physics2D.OverlapBox(checkPosition, boxSize, 0f, wallLayer);

        if (wallCollider == null)
        {
            if (!player.isCrouching ^ player.isStandingUp)
            {
                // ���� ����� �����������, ���������� ������ ����������
                playerLadderMovement.PlayerColliderResize(false);
            }
        }
    }

    private void OnDrawGizmos()
    {
        // ������������ ������� ��������
        Gizmos.color = Color.red;
        Vector2 checkPosition = (Vector2)transform.position + boxOffset;
        Gizmos.DrawWireCube(checkPosition, boxSize);
    }
}
