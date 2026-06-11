using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles player tool input, aim direction, mining hit detection, and simple monster attacks.
/// </summary>
public class ToolController : MonoBehaviour
{
    [Header("Tool Settings")]
    [SerializeField] private ToolData currentTool;
    [SerializeField] private int toolLevel = 1;
    [SerializeField] private float toolEfficiency = 1f;
    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private float attackOffset = 0.75f;
    [SerializeField] private LayerMask tilemapLayer;
    [SerializeField] private string[] monsterTargetTags = { "Monster", "Enemy" };

    private Player player;
    private Animator playerAnimator;
    private SpriteRenderer playerRenderer;
    private MiningController miningController;
    private Collider2D playerCollider;
    private Camera mainCamera;
    private Vector2 queuedAttackDirection = Vector2.down;
    private readonly List<MonsterHealth> damagedMonsters = new List<MonsterHealth>();

    private void OnEnable()
    {
        EnsureToolVisualVisible();
    }

    private void Start()
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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TriggerAttack();
        }
    }

    private void TriggerAttack()
    {
        // Starting an attack also starts the run-card timer because the player has acted.
        RunCardManager.Instance.RegisterCardTimerStartAction();

        queuedAttackDirection = ResolveAimDirection();
        if (player != null)
        {
            player.SetFacingDirection(queuedAttackDirection);
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Mine");
        }
    }

    public void CheckActionHit()
    {
        // Called from the player animation so the hit lands when the swing visually connects.
        if (player == null)
        {
            player = GetComponentInParent<Player>();
        }

        Vector2 facingDir = queuedAttackDirection;
        Vector3 hitCenter = GetAttackOrigin() + new Vector3(facingDir.x, facingDir.y, 0f) * attackOffset;

        if (miningController == null)
        {
            miningController = FindFirstObjectByType<MiningController>();
        }

        if (miningController != null)
        {
            Collider2D hitCollider = Physics2D.OverlapCircle(hitCenter, attackRadius, tilemapLayer);
            if (hitCollider != null)
            {
                Vector2 resolvedHitPoint = hitCollider.ClosestPoint(hitCenter);
                miningController.TryMineAtPosition(resolvedHitPoint, CurrentToolLevel, CurrentToolEfficiency);
            }
        }

        CheckMonsterHit(hitCenter);
    }

    public void EquipTool(ToolData tool)
    {
        currentTool = tool;
        EnsureToolVisualVisible();
    }

    public ToolData CurrentTool => currentTool;

    public int CurrentToolLevel => Mathf.Max(currentTool != null ? currentTool.MiningPower : toolLevel, PlayerUpgradeState.ToolMiningPower);

    public float CurrentToolEfficiency => Mathf.Max(0.01f, currentTool != null ? currentTool.StaminaEfficiency : toolEfficiency) * PlayerUpgradeState.ToolStaminaMultiplier;

    private void CheckMonsterHit(Vector3 hitCenter)
    {
        // Track damaged monsters so one swing cannot hit the same enemy through multiple colliders.
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(hitCenter, attackRadius);
        if (hitColliders == null || hitColliders.Length == 0)
        {
            return;
        }

        damagedMonsters.Clear();
        for (int i = 0; i < hitColliders.Length; i++)
        {
            Collider2D hit = hitColliders[i];
            if (hit == null || (player != null && hit.gameObject == player.gameObject))
            {
                continue;
            }

            GameObject monsterRoot = FindMonsterRoot(hit.gameObject);
            MonsterHealth monsterHealth = hit.GetComponentInParent<MonsterHealth>();
            if (monsterHealth == null && monsterRoot != null)
            {
                monsterHealth = monsterRoot.AddComponent<MonsterHealth>();
            }

            if (monsterHealth == null || damagedMonsters.Contains(monsterHealth))
            {
                continue;
            }

            damagedMonsters.Add(monsterHealth);
            monsterHealth.TakeDamage(PlayerUpgradeState.WeaponDamage);
        }
    }

    private GameObject FindMonsterRoot(GameObject target)
    {
        if (target == null || monsterTargetTags == null)
        {
            return null;
        }

        Transform current = target.transform;
        while (current != null)
        {
            string targetTag = current.gameObject.tag;
            for (int i = 0; i < monsterTargetTags.Length; i++)
            {
                if (targetTag == monsterTargetTags[i])
                {
                    return current.gameObject;
                }
            }

            current = current.parent;
        }

        return null;
    }

    private void OnDrawGizmosSelected()
    {
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
        // Mouse aim is converted to cardinal facing to match the four-direction animation set.
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
        // Keep tool sprites visible above the player even when prefabs are enabled dynamically.
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
