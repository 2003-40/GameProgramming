using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class SimpleMonsterMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.2f;
    [SerializeField] private float chaseRange = 4f;
    [SerializeField] private float stopDistance = 0.2f;
    [SerializeField] private float patrolDistance = 2f;
    [SerializeField] private int flyingSortingOrder = 5;

    private Rigidbody2D rb2D;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Player player;
    private Vector2 startPosition;
    private int patrolDirection = 1;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        startPosition = transform.position;

        ConfigureRigidbody();
        ConfigureFlyingVisuals();
        EnsureColliderSize();
    }

    private void Start()
    {
        player = FindFirstObjectByType<Player>();
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(rb2D.position, player.transform.position);
            if (distanceToPlayer <= chaseRange && distanceToPlayer > stopDistance)
            {
                ChasePlayer();
                return;
            }
        }

        Patrol();
    }

    private void ChasePlayer()
    {
        Vector2 direction = ((Vector2)player.transform.position - rb2D.position).normalized;
        Move(direction);
    }

    private void Patrol()
    {
        if (patrolDistance <= 0f)
        {
            return;
        }

        float patrolOffset = rb2D.position.x - startPosition.x;
        if (patrolDirection > 0 && patrolOffset >= patrolDistance)
        {
            patrolDirection = -1;
        }
        else if (patrolDirection < 0 && patrolOffset <= -patrolDistance)
        {
            patrolDirection = 1;
        }

        Move(Vector2.right * patrolDirection);
    }

    private void Move(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Vector2 normalizedDirection = direction.normalized;
        rb2D.MovePosition(rb2D.position + normalizedDirection * moveSpeed * Time.fixedDeltaTime);
        UpdateAnimator(normalizedDirection, moveSpeed);
    }

    private void UpdateAnimator(Vector2 direction, float speed)
    {
        if (animator == null)
        {
            return;
        }

        Vector2 facingDirection = GetPrimaryDirection(direction);
        animator.SetFloat("Horizontal", facingDirection.x);
        animator.SetFloat("Vertical", facingDirection.y);
        animator.SetFloat("Speed", speed);
    }

    private static Vector2 GetPrimaryDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.0001f)
        {
            return Vector2.down;
        }

        return Mathf.Abs(direction.x) > Mathf.Abs(direction.y)
            ? new Vector2(Mathf.Sign(direction.x), 0f)
            : new Vector2(0f, Mathf.Sign(direction.y));
    }

    private void ConfigureRigidbody()
    {
        rb2D.gravityScale = 0f;
        rb2D.freezeRotation = true;
    }

    private void ConfigureFlyingVisuals()
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].sortingOrder = flyingSortingOrder;
        }
    }

    private void EnsureColliderSize()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null || (boxCollider.size.x >= 0.05f && boxCollider.size.y >= 0.05f))
        {
            return;
        }

        Vector2 colliderSize = new Vector2(0.75f, 0.9f);
        Vector2 colliderOffset = Vector2.zero;
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            Bounds spriteBounds = spriteRenderer.sprite.bounds;
            colliderSize = spriteBounds.size;
            colliderOffset = spriteBounds.center;
        }

        boxCollider.size = new Vector2(Mathf.Max(0.35f, colliderSize.x * 0.8f), Mathf.Max(0.35f, colliderSize.y * 0.8f));
        boxCollider.offset = colliderOffset;
        Debug.LogWarning($"[Monster Setup] {name} had an extremely small BoxCollider2D, so it was resized to {boxCollider.size}.");
    }

    private void OnValidate()
    {
        moveSpeed = Mathf.Max(0f, moveSpeed);
        chaseRange = Mathf.Max(0f, chaseRange);
        stopDistance = Mathf.Max(0f, stopDistance);
        patrolDistance = Mathf.Max(0f, patrolDistance);
    }
}
