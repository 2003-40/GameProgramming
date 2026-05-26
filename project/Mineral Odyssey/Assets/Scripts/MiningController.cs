using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MiningController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap oreTilemap;    
    [SerializeField] private Camera mainCamera;    
    [SerializeField] private Player playerScript; 

    [Header("Mining Config")]
    [SerializeField] private float miningRange = 1.5f; 
    [SerializeField] [Range(-1f, 1f)] private float miningAngleCos = 0.5f; // 稍微调低到0.5（约前方120度），容错率更高，手感更好
    [SerializeField] private int playerToolLevel = 1; 

    [Header("Juice Config")]
    [SerializeField] private float bounceForce = 4f; 

    private Dictionary<Vector3Int, int> oreHealthTracker = new Dictionary<Vector3Int, int>();
    private bool isWobbling = false; 
    private Collider2D playerCollider;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (oreTilemap == null)
        {
            oreTilemap = GameObject.Find("OreTilemap")?.GetComponent<Tilemap>();
        }
        
        if (playerScript == null)
        {
            playerScript = GetComponent<Player>() ?? GetComponentInParent<Player>() ?? FindFirstObjectByType<Player>();
        }

        if (playerScript != null)
        {
            playerCollider = playerScript.GetComponent<Collider2D>();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ProcessMining();
        }
    }

    private void ProcessMining()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector3Int gridPos = oreTilemap.WorldToCell(mouseWorldPos);
        TileBase clickedTile = oreTilemap.GetTile(gridPos);

        if (clickedTile is MiningTile currentOre)
        {
            // --------- 【修复 Bug 1：防卡死/脱卡机制】 ---------
            bool isPlayerInsideTile = false;
            if (playerCollider != null)
            {
                // 计算当前格子的世界范围边界
                Vector3 cellCenter = oreTilemap.GetCellCenterWorld(gridPos);
                Vector3 cellSize = oreTilemap.cellSize;
                Bounds tileBounds = new Bounds(cellCenter, cellSize);

                // 如果玩家碰撞体和石头格子重合了
                if (playerCollider.bounds.Intersects(tileBounds))
                {
                    isPlayerInsideTile = true; // 标记重合
                }
            }

            // 如果没有被压住，正常执行距离和朝向判定
            if (!isPlayerInsideTile)
            {
                // 1.1 限制挖掘距离
                Vector3 directionToTarget = mouseWorldPos - transform.position;
                float distance = directionToTarget.magnitude;

                if (distance > miningRange)
                {
                    Debug.Log("太远了，手不够长！");
                    return;
                }

                // 1.1 面朝向检查（通过动画状态机获取精准记忆方向）
                Vector3 playerForward = Vector3.up; 
                if (playerScript != null)
                {
                    Vector2 facing = playerScript.GetFacingDirection();
                    playerForward = new Vector3(facing.x, facing.y, 0f);
                }

                directionToTarget.Normalize();
                float dotProduct = Vector3.Dot(playerForward, directionToTarget);

                if (dotProduct < miningAngleCos)
                {
                    Debug.Log("你没有面对着矿石！");
                    return;
                }
            }
            else
            {
                Debug.LogWarning("玩家被矿石卡住了！触发紧急脱卡挖掘，无视距离与朝向约束！");
            }

            // 3.2 挖掘工具等级限制
            if (playerToolLevel < currentOre.requiredToolLevel)
            {
                Debug.Log($"[弹刀！] 工具等级不足！");
                return;
            }

            HandleDamage(gridPos, currentOre);
        }
    }

    private void HandleDamage(Vector3Int gridPos, MiningTile ore)
    {
        if (!oreHealthTracker.ContainsKey(gridPos))
        {
            oreHealthTracker.Add(gridPos, ore.maxHealth);
        }

        oreHealthTracker[gridPos]--;
        Vector3 cellWorldPos = oreTilemap.GetCellCenterWorld(gridPos);

        // --------- 【修复 Bug 3：受击粒子每次都触发并自动销毁】 ---------
        if (ore.hitParticlePrefab != null)
        {
            GameObject particleObj = Instantiate(ore.hitParticlePrefab.gameObject, cellWorldPos, Quaternion.identity);
            // 提醒：确保粒子Prefab的 Main 模块中 Loop 是关闭的，Play On Awake 是开启的
            ParticleSystem ps = particleObj.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
                // 按照粒子的生存周期延迟自动销毁该特效物体，防止内存泄漏或残留
                Destroy(particleObj, ps.main.duration + ps.main.startLifetime.constantMax);
            }
        }

        // 2.1 矿石受击震动效果
        if (!isWobbling)
        {
            StartCoroutine(WobbleTilemapVisual());
        }

        // 3.1 矿石多阶段视觉变化
        float healthPercent = (float)oreHealthTracker[gridPos] / ore.maxHealth;
        Color damageColor = Color.Lerp(Color.gray, Color.white, healthPercent); 
        
        oreTilemap.SetTileFlags(gridPos, TileFlags.None); 
        oreTilemap.SetColor(gridPos, damageColor);

        if (oreHealthTracker[gridPos] <= 0)
        {
            ExecuteDestruction(gridPos, ore);
        }
    }

    IEnumerator WobbleTilemapVisual()
    {
        isWobbling = true;
        Vector3 originalPos = oreTilemap.transform.position;
        float elapsed = 0f;
        float duration = 0.1f;
        float magnitude = 0.05f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            oreTilemap.transform.position = originalPos + new Vector3(x, y, 0);
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        oreTilemap.transform.position = originalPos;
        isWobbling = false;
    }

    private void ExecuteDestruction(Vector3Int gridPos, MiningTile ore)
    {
        oreTilemap.SetColor(gridPos, Color.white);
        oreTilemap.SetTileFlags(gridPos, TileFlags.LockColor);
        oreTilemap.SetTile(gridPos, null);

        Vector3 spawnPosition = oreTilemap.GetCellCenterWorld(gridPos);

        if (ore.dropPrefab != null)
        {
            GameObject droppedItem = Instantiate(ore.dropPrefab, spawnPosition, Quaternion.identity);
            Rigidbody2D rb = droppedItem.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                rb.AddForce(randomDirection * bounceForce, ForceMode2D.Impulse);
            }
        }

        oreHealthTracker.Remove(gridPos);
    }
}