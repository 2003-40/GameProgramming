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

        movementInput = movementInput.normalized;

        animator.SetFloat("Horizontal", movementInput.x);
        animator.SetFloat("Vertical", movementInput.y);
        animator.SetFloat("Speed", movementInput.magnitude); // 速度参数，控制动画切换（比如从站立到行走）
    }

    private void FixedUpdate()
    {
        // 办法 A：直接用旧版的 velocity 控速移动（最常用）
        rb2D.velocity = movementInput * speed;

        // 办法 B：如果你想用 MovePosition，就把上面那行 rb2D.velocity 删掉，用下面这行：
        // rb2D.MovePosition(rb2D.position + movementInput * speed * Time.fixedDeltaTime);
    }
}