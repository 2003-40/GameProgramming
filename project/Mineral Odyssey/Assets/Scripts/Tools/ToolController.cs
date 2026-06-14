using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

/// <summary>
/// Handles player tool input, aim direction, mining hit detection, and simple monster attacks.
/// </summary>
public class ToolController : MonoBehaviour
{
    private const int MiningMouseButton = 0;
    private const int WeaponMouseButton = 1;
    private const string LevelOneSceneName = "FirstFlour";

    private enum ToolActionMode
    {
        None,
        Weapon,
        Mining
    }

    [Header("Tool Settings")]
    [SerializeField] private ToolData currentTool;
    [SerializeField] private int toolLevel = 1;
    [SerializeField] private float toolEfficiency = 1f;
    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private float attackOffset = 0.75f;
    [SerializeField] private string[] monsterTargetTags = { "Monster", "Enemy" };
    [SerializeField] private float closeMiningFallbackRadiusMultiplier = 0.75f;
    [SerializeField] private int minimumToolSortingOrder = 3;
    [SerializeField] private float miningToolVisibleDuration = 0.42f;

    [Header("Weapon Visual")]
    [SerializeField] private Transform weaponAnchor;
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;
    [SerializeField] private Sprite copperWeaponSprite;
    [SerializeField] private Sprite ironWeaponSprite;
    [SerializeField] private Sprite crystalWeaponSprite;
    [SerializeField] private float weaponThrustDistance = 0.45f;
    [SerializeField] private float weaponThrustOutDuration = 0.08f;
    [SerializeField] private float weaponThrustReturnDuration = 0.1f;
    [SerializeField] private string[] weaponHiddenSceneNames = { LevelOneSceneName };

    private Player player;
    private Animator playerAnimator;
    private SpriteRenderer playerRenderer;
    private MiningController miningController;
    private Collider2D playerCollider;
    private Camera mainCamera;
    private Vector2 queuedAttackDirection = Vector2.down;
    private Vector2 queuedFallbackMiningDirection = Vector2.down;
    private bool hasPendingActionHit;
    private ToolActionMode pendingActionMode = ToolActionMode.None;
    private Vector3 weaponAnchorRestLocalPosition;
    private Coroutine miningToolVisualRoutine;
    private Coroutine weaponThrustRoutine;
    private bool isMiningToolVisible;
    private bool isWeaponVisualVisible;
    private int cachedHighestTilemapSortingOrder = int.MinValue;
    private readonly List<MonsterHealth> damagedMonsters = new List<MonsterHealth>();

    private void OnEnable()
    {
        PlayerUpgradeState.UpgradesChanged += ApplyWeaponTierSprite;
        RefreshCarriedVisuals();
        ApplyWeaponTierSprite();
    }

    private void OnDisable()
    {
        PlayerUpgradeState.UpgradesChanged -= ApplyWeaponTierSprite;
        isMiningToolVisible = false;
        isWeaponVisualVisible = false;
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

        RefreshCarriedVisuals();
        CacheWeaponVisual();
        ApplyWeaponTierSprite();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(WeaponMouseButton))
        {
            TriggerAttack(ToolActionMode.Weapon);
        }
        else if (Input.GetMouseButtonDown(MiningMouseButton))
        {
            TriggerAttack(ToolActionMode.Mining);
        }
    }

    private void LateUpdate()
    {
        RefreshCarriedVisuals();
    }

    private void TriggerAttack(ToolActionMode actionMode)
    {
        if (actionMode == ToolActionMode.Weapon && !IsWeaponAvailable())
        {
            return;
        }

        // Starting an attack also starts the run-card timer because the player has acted.
        RunCardManager.Instance.RegisterCardTimerStartAction();

        Vector2 aimDirection = ResolveAimDirection();
        if (actionMode == ToolActionMode.Mining)
        {
            queuedAttackDirection = player != null ? GetCardinalFacing(player.GetFacingDirection()) : aimDirection;
            queuedFallbackMiningDirection = aimDirection;
        }
        else
        {
            queuedAttackDirection = aimDirection;
            queuedFallbackMiningDirection = queuedAttackDirection;
        }

        hasPendingActionHit = true;
        pendingActionMode = actionMode;
        if (player != null)
        {
            player.SetFacingDirection(queuedAttackDirection);
        }

        if (actionMode == ToolActionMode.Mining)
        {
            PlayMiningToolVisual();
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger("Mine");
            }
        }
        else if (actionMode == ToolActionMode.Weapon)
        {
            PlayWeaponThrust();
        }

        CheckActionHit();
    }

    public void CheckActionHit()
    {
        if (!hasPendingActionHit)
        {
            return;
        }

        ToolActionMode actionMode = pendingActionMode;
        hasPendingActionHit = false;
        pendingActionMode = ToolActionMode.None;
        PerformActionHit(actionMode);
    }

    private void PerformActionHit(ToolActionMode actionMode)
    {
        // Called from the player animation so the hit lands when the swing visually connects.
        if (player == null)
        {
            player = GetComponentInParent<Player>();
        }

        Vector2 facingDir = queuedAttackDirection;
        Vector3 hitCenter = GetAttackOrigin() + new Vector3(facingDir.x, facingDir.y, 0f) * attackOffset;
        Debug.Log($"[Tool Action] Mode={actionMode}, Direction={facingDir}, HitCenter={hitCenter}");

        if (actionMode == ToolActionMode.Mining)
        {
            if (miningController == null)
            {
                miningController = FindFirstObjectByType<MiningController>();
            }

            if (miningController != null)
            {
                bool mined = miningController.TryMineNearPosition(hitCenter, attackRadius, CurrentToolLevel, CurrentToolEfficiency);
                if (!mined && queuedFallbackMiningDirection != facingDir)
                {
                    Vector3 fallbackHitCenter = GetAttackOrigin() + new Vector3(queuedFallbackMiningDirection.x, queuedFallbackMiningDirection.y, 0f) * attackOffset;
                    mined = miningController.TryMineNearPosition(fallbackHitCenter, attackRadius, CurrentToolLevel, CurrentToolEfficiency);
                }

                if (!mined)
                {
                    float closeRadius = attackRadius * Mathf.Max(0.75f, closeMiningFallbackRadiusMultiplier);
                    miningController.TryMineNearPosition(GetAttackOrigin(), closeRadius, CurrentToolLevel, CurrentToolEfficiency);
                }
            }
        }

        if (actionMode == ToolActionMode.Weapon)
        {
            CheckMonsterHit(hitCenter);
        }
    }

    public void EquipTool(ToolData tool)
    {
        currentTool = tool;
        RefreshCarriedVisuals();
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

    private void CacheWeaponVisual()
    {
        if (weaponAnchor == null && player != null)
        {
            Transform playerTransform = player.transform;
            weaponAnchor = playerTransform.Find("WeaponAnchor");
        }

        if (weaponAnchor != null)
        {
            weaponAnchorRestLocalPosition = weaponAnchor.localPosition;
        }

        if (weaponSpriteRenderer == null && weaponAnchor != null)
        {
            Transform weaponSpriteTransform = weaponAnchor.Find("WeaponSprite");
            if (weaponSpriteTransform != null)
            {
                weaponSpriteRenderer = weaponSpriteTransform.GetComponent<SpriteRenderer>();
            }

            if (weaponSpriteRenderer == null)
            {
                weaponSpriteRenderer = weaponAnchor.GetComponentInChildren<SpriteRenderer>(true);
            }
        }
    }

    private void ApplyWeaponTierSprite()
    {
        CacheWeaponVisual();

        if (weaponSpriteRenderer == null)
        {
            return;
        }

        Sprite tierSprite = GetWeaponTierSprite(PlayerUpgradeState.WeaponLevel);
        if (tierSprite != null)
        {
            weaponSpriteRenderer.sprite = tierSprite;
        }
    }

    private Sprite GetWeaponTierSprite(int weaponLevel)
    {
        switch (Mathf.Clamp(weaponLevel, PlayerUpgradeState.MinWeaponLevel, PlayerUpgradeState.MaxWeaponLevel))
        {
            case 1:
                return copperWeaponSprite;
            case 2:
                return ironWeaponSprite;
            case 3:
                return crystalWeaponSprite;
            default:
                return copperWeaponSprite;
        }
    }

    private void PlayWeaponThrust()
    {
        if (!IsWeaponAvailable())
        {
            return;
        }

        if (weaponThrustRoutine != null)
        {
            StopCoroutine(weaponThrustRoutine);
            weaponAnchor.localPosition = weaponAnchorRestLocalPosition;
        }

        isWeaponVisualVisible = true;
        RefreshCarriedVisuals();
        weaponThrustRoutine = StartCoroutine(AnimateWeaponThrust(queuedAttackDirection));
    }

    private bool IsWeaponAvailable()
    {
        if (weaponAnchor == null)
        {
            CacheWeaponVisual();
        }

        return IsWeaponVisibleForCurrentScene() && weaponAnchor != null && weaponAnchor.gameObject.activeInHierarchy;
    }

    private System.Collections.IEnumerator AnimateWeaponThrust(Vector2 direction)
    {
        Vector2 facing = GetCardinalFacing(direction);
        Vector3 thrustOffset = new Vector3(facing.x, facing.y, 0f) * Mathf.Max(0f, weaponThrustDistance);
        Vector3 thrustPosition = weaponAnchorRestLocalPosition + thrustOffset;

        yield return MoveWeaponAnchor(weaponAnchorRestLocalPosition, thrustPosition, weaponThrustOutDuration);
        yield return MoveWeaponAnchor(thrustPosition, weaponAnchorRestLocalPosition, weaponThrustReturnDuration);

        weaponAnchor.localPosition = weaponAnchorRestLocalPosition;
        weaponThrustRoutine = null;
        isWeaponVisualVisible = false;
        RefreshCarriedVisuals();
    }

    private System.Collections.IEnumerator MoveWeaponAnchor(Vector3 startPosition, Vector3 endPosition, float duration)
    {
        if (duration <= 0f)
        {
            weaponAnchor.localPosition = endPosition;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            weaponAnchor.localPosition = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        weaponAnchor.localPosition = endPosition;
    }

    private void PlayMiningToolVisual()
    {
        if (miningToolVisualRoutine != null)
        {
            StopCoroutine(miningToolVisualRoutine);
        }

        isMiningToolVisible = true;
        RefreshCarriedVisuals();
        miningToolVisualRoutine = StartCoroutine(HideMiningToolAfterUse());
    }

    private System.Collections.IEnumerator HideMiningToolAfterUse()
    {
        yield return new WaitForSeconds(Mathf.Max(0.05f, miningToolVisibleDuration));
        isMiningToolVisible = false;
        miningToolVisualRoutine = null;
        RefreshCarriedVisuals();
    }

    private void RefreshCarriedVisuals()
    {
        // Keep the controller object active, but only render carried items while they are being used.
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        int sortingOrder = Mathf.Max(3, minimumToolSortingOrder);
        string sortingLayerName = string.Empty;

        if (playerRenderer == null)
        {
            playerRenderer = GetComponentInParent<SpriteRenderer>();
        }

        if (playerRenderer != null)
        {
            sortingOrder = Mathf.Max(sortingOrder, playerRenderer.sortingOrder + 1);
            sortingLayerName = playerRenderer.sortingLayerName;
        }

        sortingOrder = Mathf.Max(sortingOrder, GetHighestTilemapSortingOrder() + 1);

        ApplyCarriedVisualState(transform, sortingOrder, sortingLayerName, isMiningToolVisible, true);

        if (weaponAnchor == null)
        {
            CacheWeaponVisual();
        }

        if (weaponAnchor != null)
        {
            ApplyCarriedVisualState(weaponAnchor, sortingOrder, sortingLayerName, isWeaponVisualVisible && IsWeaponVisibleForCurrentScene(), true);
        }
    }

    private bool IsWeaponVisibleForCurrentScene()
    {
        string activeSceneName = SceneManager.GetActiveScene().name;
        if (activeSceneName == LevelOneSceneName)
        {
            return false;
        }

        if (weaponHiddenSceneNames == null)
        {
            return true;
        }

        for (int i = 0; i < weaponHiddenSceneNames.Length; i++)
        {
            if (activeSceneName == weaponHiddenSceneNames[i])
            {
                return false;
            }
        }

        return true;
    }

    private int GetHighestTilemapSortingOrder()
    {
        if (cachedHighestTilemapSortingOrder != int.MinValue)
        {
            return cachedHighestTilemapSortingOrder;
        }

        cachedHighestTilemapSortingOrder = 0;
        TilemapRenderer[] tilemapRenderers = FindObjectsByType<TilemapRenderer>(FindObjectsSortMode.None);
        for (int i = 0; i < tilemapRenderers.Length; i++)
        {
            cachedHighestTilemapSortingOrder = Mathf.Max(cachedHighestTilemapSortingOrder, tilemapRenderers[i].sortingOrder);
        }

        return cachedHighestTilemapSortingOrder;
    }

    private static void ApplyCarriedVisualState(Transform visualRoot, int sortingOrder, string sortingLayerName, bool visible, bool keepRootActive)
    {
        if (visualRoot == null)
        {
            return;
        }

        visualRoot.gameObject.SetActive(keepRootActive || visible);

        SpriteRenderer[] toolRenderers = visualRoot.GetComponentsInChildren<SpriteRenderer>(true);
        for (int i = 0; i < toolRenderers.Length; i++)
        {
            SpriteRenderer toolRenderer = toolRenderers[i];
            toolRenderer.gameObject.SetActive(visible);
            toolRenderer.enabled = visible;
            if (!visible)
            {
                continue;
            }

            toolRenderer.sortingOrder = sortingOrder;

            if (!string.IsNullOrWhiteSpace(sortingLayerName))
            {
                toolRenderer.sortingLayerName = sortingLayerName;
            }
        }
    }
}
