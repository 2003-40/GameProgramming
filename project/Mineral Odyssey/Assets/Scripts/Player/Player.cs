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
    
    // 【核心修复】记忆最后一次有效的移动输入，默认朝下
    private Vector2 lastValidFacing = Vector2.down; 

    void Start()
    {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        toolController = GetComponentInChildren<ToolController>();
        
        // 初始同步一次动画机参数
        UpdateAnimatorParams(lastValidFacing, 0f);
    }

    void Update()
    {
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        if (movementInput.magnitude > 0.01f)
        {
            // 归一化防止斜向走变快
            lastValidFacing = movementInput.normalized;
            
            // 移动时同步更新动画机
            UpdateAnimatorParams(lastValidFacing, movementInput.magnitude);
        }
        else
        {
            // 【核心修复】停止移动时，速度传 0 切换到 Idle，但方向参数强制锁死
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
    /// 供采矿、攻击等外部脚本调用的绝对同步朝向
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
