using UnityEngine;

public class WallLadderDetector : MonoBehaviour
{
    private LadderMovement playerLadderMovement;
    private PlatformerPlayer player;
    public LayerMask wallLayer; // Слой для стен
    public Vector2 boxSize = new Vector2(1.5f, 3.825f); // Размер области проверки
    public Vector2 boxOffset = new Vector2(0f, -0.275f); // Смещение области проверки

    private void Start()
    {
        player = GetComponentInParent<PlatformerPlayer>();
        playerLadderMovement = GetComponentInParent<LadderMovement>();
    }
    void Update()
    {
        // Позиция центра проверки
        Vector2 checkPosition = (Vector2)transform.position + boxOffset;

        // Проверяем наличие стен в области
        Collider2D wallCollider = Physics2D.OverlapBox(checkPosition, boxSize, 0f, wallLayer);

        if (wallCollider == null)
        {
            if (!player.isCrouching ^ player.isStandingUp)
            {
                // Если стена отсутствует, возвращаем размер коллайдера
                playerLadderMovement.PlayerColliderResize(false);
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Визуализация области проверки
        Gizmos.color = Color.red;
        Vector2 checkPosition = (Vector2)transform.position + boxOffset;
        Gizmos.DrawWireCube(checkPosition, boxSize);
    }
}
