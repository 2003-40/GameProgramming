using UnityEngine;

public class ToolController : MonoBehaviour
{
    [Header("Tool Settings")]
    [SerializeField] private ToolData currentTool;
    [SerializeField] private int toolLevel = 1;
    [SerializeField] private float toolEfficiency = 1f;
    [SerializeField] private float attackRadius = 1f;   // 挥砍检测半径
    [SerializeField] private float attackOffset = 0.75f;   // 检测圈在角色前方的偏移量
    [SerializeField] private LayerMask tilemapLayer;      // 矿石网格所在的图层

    private Player player;
    private Animator playerAnimator;
    private SpriteRenderer playerRenderer;
    private MiningController miningController;
    private Collider2D playerCollider;
    private Camera mainCamera;
    private Vector2 queuedAttackDirection = Vector2.down;

    private void OnEnable()
    {
        EnsureToolVisualVisible();
    }

    void Start()
    {
        player = GetComponentInParent<Player>();
        playerAnimator = GetComponentInParent<Animator>();
        playerRenderer = GetComponentInParent<SpriteRenderer>();
        miningController = FindFirstObjectByType<MiningController>();
        mainCamera = Camera.main;
        if (player != null)
        {
            playerCollider = player.GetComponent<Collider2D>();
        }

        EnsureToolVisualVisible();
    }

    void Update()
    {
        // 改为按键挥舞工具，不再全屏盲点
        if (Input.GetMouseButtonDown(0))
        {
            TriggerAttack();
        }
    }

    private void TriggerAttack()
    {
        queuedAttackDirection = ResolveAimDirection();
        if (player != null)
        {
            player.SetFacingDirection(queuedAttackDirection);
        }

        if (playerAnimator != null)
        {
            // 触发玩家的挥镐动画状态（请确保动画机里有对应的 "Mine" Trigger）
            playerAnimator.SetTrigger("Mine");
        }
        
        // 注意：推荐在动画的“落镐关键帧”通过 Animation Event 调用以下检测逻辑。
        // 如果想省事直接点击生效，也可以留在这里。下面以点击即触发为例：
    }

    /// <summary>
    /// 【物理判定框取代鼠标全屏点】
    /// 供动画事件（Animation Event）调用，实现手落石开的节奏感
    /// </summary>
    public void CheckActionHit()
    {
        if (player == null || miningController == null) return;

        // 根据玩家绝对锁定的面朝向，计算前方的物理检测圆心
        Vector2 facingDir = queuedAttackDirection;
        Vector3 hitCenter = GetAttackOrigin() + new Vector3(facingDir.x, facingDir.y, 0f) * attackOffset;

        // 检测前方区域内是否存在矿石
        Collider2D hitCollider = Physics2D.OverlapCircle(hitCenter, attackRadius, tilemapLayer);
        if (hitCollider != null)
        {
            // 将碰撞点转换为网格坐标传递给采矿管理器
            Vector2 resolvedHitPoint = hitCollider.ClosestPoint(hitCenter);
            miningController.TryMineAtPosition(resolvedHitPoint, CurrentToolLevel, CurrentToolEfficiency);
        }
    }

    public void EquipTool(ToolData tool)
    {
        currentTool = tool;
        EnsureToolVisualVisible();
    }

    public ToolData CurrentTool => currentTool;

    public int CurrentToolLevel => currentTool != null ? currentTool.MiningPower : Mathf.Max(1, toolLevel);

    public float CurrentToolEfficiency => currentTool != null ? currentTool.StaminaEfficiency : Mathf.Max(0.01f, toolEfficiency);

    private void OnDrawGizmosSelected()
    {
        // 方便在编辑器里可视化挥砍距离
        if (player != null)
        {
            Vector2 facingDir = queuedAttackDirection.sqrMagnitude > 0.0001f
                ? queuedAttackDirection
                : GetCardinalFacing(player.GetFacingDirection());
            Vector3 hitCenter = GetAttackOrigin() + new Vector3(facingDir.x, facingDir.y, 0f) * attackOffset;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hitCenter, attackRadius);
        }
    }

    private Vector3 GetAttackOrigin()
    {
        if (playerCollider == null && player != null)
        {
            playerCollider = player.GetComponent<Collider2D>();
        }

        if (playerCollider != null)
        {
            return playerCollider.bounds.center;
        }

        return player != null ? player.transform.position : transform.position;
    }

    private static Vector2 GetCardinalFacing(Vector2 rawFacing)
    {
        if (rawFacing.sqrMagnitude < 0.0001f)
        {
            return Vector2.down;
        }

        if (Mathf.Abs(rawFacing.x) > Mathf.Abs(rawFacing.y))
        {
            return new Vector2(Mathf.Sign(rawFacing.x), 0f);
        }

        return new Vector2(0f, Mathf.Sign(rawFacing.y));
    }

    private Vector2 ResolveAimDirection()
    {
        if (player == null)
        {
            return Vector2.down;
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
        {
            return GetCardinalFacing(player.GetFacingDirection());
        }

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector2 toMouse = mouseWorldPos - GetAttackOrigin();
        if (toMouse.sqrMagnitude < 0.0001f)
        {
            return GetCardinalFacing(player.GetFacingDirection());
        }

        return GetCardinalFacing(toMouse);
    }

    private void EnsureToolVisualVisible()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        SpriteRenderer[] toolRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        if (toolRenderers.Length == 0)
        {
            return;
        }

        int sortingOrder = 2;
        string sortingLayerName = string.Empty;

        if (playerRenderer == null)
        {
            playerRenderer = GetComponentInParent<SpriteRenderer>();
        }

        if (playerRenderer != null)
        {
            sortingOrder = playerRenderer.sortingOrder + 1;
            sortingLayerName = playerRenderer.sortingLayerName;
        }

        for (int i = 0; i < toolRenderers.Length; i++)
        {
            SpriteRenderer toolRenderer = toolRenderers[i];
            toolRenderer.gameObject.SetActive(true);
            toolRenderer.enabled = true;
            toolRenderer.sortingOrder = sortingOrder;

            if (!string.IsNullOrWhiteSpace(sortingLayerName))
            {
                toolRenderer.sortingLayerName = sortingLayerName;
            }
        }
    }
}
