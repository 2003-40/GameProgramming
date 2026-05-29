using UnityEngine;

public class ItemMagnet : MonoBehaviour
{
    [Header("Magnet Settings")]
    [SerializeField] private float magnetRadius = 2.0f; // 1.2 吸附范围
    [SerializeField] private float moveSpeed = 6.0f;    // 移动速度
    [SerializeField] private float activateDelay = 0.4f; // 延迟激活时间（留给2.3物理爆开）

    private Transform playerTransform;
    private Rigidbody2D rb;
    private bool isAttracted = false;
    private float timer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // 寻找玩家 (请确保你的玩家组件 Tag 是 "Player")
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // 计时器未到时，先让物理引擎接管（自由爆开弹跳）
        if (timer < activateDelay)
        {
            timer += Time.deltaTime;
            return;
        }

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // 进入磁铁范围
        if (distance <= magnetRadius)
        {
            isAttracted = true;
        }

        if (isAttracted)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
            
            // 如果距离极近，模拟捡起物体
            if (distance < 0.2f)
            {
                // TODO: 在这里接入你的背包系统，例如 PlayerInventory.Instance.Add(gemType);
                Destroy(gameObject);
            }
        }
    }
}