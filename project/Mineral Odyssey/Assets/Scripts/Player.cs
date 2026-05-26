using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody2D rb2D;
    private Vector2 movementInput;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        if (movementInput.magnitude > 0)
        {
            movementInput = movementInput.normalized;

            // 只有在走路移动时，才更新动画机的方向参数
            animator.SetFloat("Horizontal", movementInput.x);
            animator.SetFloat("Vertical", movementInput.y);
        }

        // 无论动没动，随时把速度传给动画机（用于切换 Idle 和 Walk 动画状态）
        animator.SetFloat("Speed", movementInput.magnitude); 
    }

    private void FixedUpdate()
    {
        rb2D.velocity = movementInput * speed;
    }

    /// <summary>
    /// 【新增方法】供采矿脚本调用，直接抓取动画机当前所处（或最后保留）的面朝方向
    /// </summary>
    public Vector2 GetFacingDirection()
    {
        if (animator != null)
        {
            float h = animator.GetFloat("Horizontal");
            float v = animator.GetFloat("Vertical");
            return new Vector2(h, v).normalized;
        }
        return Vector2.down; // 兜底默认朝下
    }
}