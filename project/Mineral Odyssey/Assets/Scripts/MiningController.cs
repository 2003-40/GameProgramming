using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MiningController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap oreTilemap;    

    [Header("Juice Config")]
    [SerializeField] private float bounceForce = 4f; 

    private Dictionary<Vector3Int, int> oreHealthTracker = new Dictionary<Vector3Int, int>();
    private bool isWobbling = false; 

    void Start()
    {
        if (oreTilemap == null)
        {
            oreTilemap = GameObject.Find("OreTilemap")?.GetComponent<Tilemap>();
        }

        // --------- 【优化修复 Bug 2：出生/刷新时单次排查卡死】 ---------
        ResolveInitialStuckPlayers();
    }

    /// <summary>
    /// 一次性检测，防止游戏刚加载或者矿石生成时把玩家卡在里面
    /// </summary>
    private void ResolveInitialStuckPlayers()
    {
        Player player = FindFirstObjectByType<Player>();
        if (player == null || oreTilemap == null) return;

        Collider2D playerCol = player.GetComponent<Collider2D>();
        if (playerCol == null) return;

        // 获取玩家当前的中心坐标
        Vector3Int startGrid = oreTilemap.WorldToCell(player.transform.position);
        
        // 检索周围 3x3 范围的格子
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int checkPos = startGrid + new Vector3Int(x, y, 0);
                if (oreTilemap.HasTile(checkPos))
                {
                    Vector3 cellCenter = oreTilemap.GetCellCenterWorld(checkPos);
                    Bounds tileBounds = new Bounds(cellCenter, oreTilemap.cellSize);

                    // 如果出生时重叠了
                    if (playerCol.bounds.Intersects(tileBounds))
                    {
                        Debug.LogWarning($"[安全启动] 玩家出生在矿石 {checkPos} 内部！执行单次清开处理。");
                        oreTilemap.SetTile(checkPos, null); // 移除该危险矿石
                    }
                }
            }
        }
    }

    /// <summary>
    /// 【纯净化重构】供工具脚本检测到碰撞后直接调用
    /// </summary>
    /// <param name="worldHitPos">挥砍命中的世界坐标点</param>
    /// <param name="incomingToolLevel">当前玩家手持工具的级别</param>
    public void TryMineAtPosition(Vector3 worldHitPos, int incomingToolLevel)
    {
        if (oreTilemap == null) return;

        Vector3Int gridPos = oreTilemap.WorldToCell(worldHitPos);
        TileBase clickedTile = oreTilemap.GetTile(gridPos);

        if (clickedTile is MiningTile currentOre)
        {
            // 验证工具等级
            if (incomingToolLevel < currentOre.requiredToolLevel)
            {
                Debug.Log($"[弹刀！] 工具等级 {incomingToolLevel} 低于矿石所需等级 {currentOre.requiredToolLevel}");
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

        // 受击粒子触发与销毁
        if (ore.hitParticlePrefab != null)
        {
            GameObject particleObj = Instantiate(ore.hitParticlePrefab.gameObject, cellWorldPos, Quaternion.identity);
            ParticleSystem ps = particleObj.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
                Destroy(particleObj, ps.main.duration + ps.main.startLifetime.constantMax);
            }
        }

        // 矿石受击震动
        if (!isWobbling)
        {
            StartCoroutine(WobbleTilemapVisual());
        }

        // 多阶段视觉碎裂/褪色变化
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