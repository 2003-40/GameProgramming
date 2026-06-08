using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;

    [Header("Stamina Damage")]
    [SerializeField] private int defaultMonsterHitStaminaDamage = 10;
    [SerializeField] private float defaultMonsterHitCooldown = 1f;
    [SerializeField] private string[] staminaDamageTags = { "Monster", "Enemy", "Hazard" };

    private Rigidbody2D rb2D;
    private Vector2 movementInput;
    private Animator animator;
    private ToolController toolController;
    private float nextStaminaDamageTime;
    
    // Remember the last valid movement input; default to facing down.
    private Vector2 lastValidFacing = Vector2.down; 

    void Start()
    {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        toolController = GetComponentInChildren<ToolController>();
        
        // Sync animator parameters once on startup.
        UpdateAnimatorParams(lastValidFacing, 0f);
    }

    void Update()
    {
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        if (movementInput.magnitude > 0.01f)
        {
            // Normalize to prevent diagonal movement from being faster.
            lastValidFacing = movementInput.normalized;
            
            // Update the animator while moving.
            UpdateAnimatorParams(lastValidFacing, movementInput.magnitude);
        }
        else
        {
            // Switch to idle while preserving the last facing direction.
            UpdateAnimatorParams(lastValidFacing, 0f);
        }
    }

    private void FixedUpdate()
    {
        rb2D.velocity = movementInput.normalized * speed;
    }

    private void UpdateAnimatorParams(Vector2 facing, float speedParam)
    {
        if (animator == null) return;
        
        animator.SetFloat("Horizontal", facing.x);
        animator.SetFloat("Vertical", facing.y);
        animator.SetFloat("Speed", speedParam);
    }

    /// <summary>
    /// Returns the facing direction used by mining, attacks, and other external scripts.
    /// </summary>
    public Vector2 GetFacingDirection()
    {
        return lastValidFacing;
    }

    public void SetFacingDirection(Vector2 facing)
    {
        if (facing.sqrMagnitude < 0.0001f)
        {
            return;
        }

        lastValidFacing = facing.normalized;
        UpdateAnimatorParams(lastValidFacing, movementInput.magnitude);
    }

    public void CheckActionHit()
    {
        if (toolController == null)
        {
            toolController = GetComponentInChildren<ToolController>();
        }

        if (toolController != null)
        {
            toolController.CheckActionHit();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryTakeStaminaDamage(collision.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryTakeStaminaDamage(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryTakeStaminaDamage(other.gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryTakeStaminaDamage(other.gameObject);
    }

    private void TryTakeStaminaDamage(GameObject damageSource)
    {
        int staminaDamage = ResolveStaminaDamage(damageSource, out float damageCooldown);
        if (staminaDamage <= 0 || Time.time < nextStaminaDamageTime)
        {
            return;
        }

        nextStaminaDamageTime = Time.time + damageCooldown;
        StaminaManager.Instance.ConsumeStamina(staminaDamage);
        string logLabel = IsMonsterDamageSource(damageSource) ? "Monster Damage" : "Stamina Damage";
        Debug.Log($"[{logLabel}] {damageSource.name} dealt {staminaDamage} stamina damage. Stamina={StaminaManager.Instance.CurrentStamina}/{StaminaManager.Instance.MaxStamina}");
    }

    private int ResolveStaminaDamage(GameObject damageSource, out float damageCooldown)
    {
        StaminaDamageOnContact staminaDamageSource = damageSource.GetComponentInParent<StaminaDamageOnContact>();
        if (staminaDamageSource != null)
        {
            damageCooldown = Mathf.Max(0f, staminaDamageSource.DamageCooldown);
            int staminaDamage = staminaDamageSource.StaminaDamage;
            return IsMonsterDamageSource(damageSource) ? PlayerUpgradeState.ReduceMonsterStaminaDamage(staminaDamage) : staminaDamage;
        }

        if (HasStaminaDamageTag(damageSource))
        {
            damageCooldown = Mathf.Max(0f, defaultMonsterHitCooldown);
            return IsMonsterDamageSource(damageSource)
                ? PlayerUpgradeState.ReduceMonsterStaminaDamage(defaultMonsterHitStaminaDamage)
                : defaultMonsterHitStaminaDamage;
        }

        damageCooldown = 0f;
        return 0;
    }

    private bool HasStaminaDamageTag(GameObject damageSource)
    {
        string damageSourceTag = damageSource.tag;
        for (int i = 0; i < staminaDamageTags.Length; i++)
        {
            if (damageSourceTag == staminaDamageTags[i])
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsMonsterDamageSource(GameObject damageSource)
    {
        Transform current = damageSource.transform;
        while (current != null)
        {
            string damageSourceTag = current.gameObject.tag;
            if (damageSourceTag == "Monster" || damageSourceTag == "Enemy")
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }
}
