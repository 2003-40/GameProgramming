using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MiningController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap oreTilemap;    

    [Header("Mining Feedback")]
    [SerializeField] private ParticleSystem fallbackHitParticlePrefab;
    [SerializeField] private ParticleSystem destroyParticlePrefab;
    [SerializeField] private MiningParticlePool hitParticlePool;
    [SerializeField] private MiningParticlePool destroyParticlePool;
    [SerializeField] private int hitParticleInitialPoolSize = 8;
    [SerializeField] private int hitParticleMaxPoolSize = 24;
    [SerializeField] private int destroyParticleInitialPoolSize = 3;
    [SerializeField] private int destroyParticleMaxPoolSize = 8;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip destroyClip;
    [Range(0f, 1f)]
    [SerializeField] private float hitVolume = 0.7f;
    [Range(0f, 1f)]
    [SerializeField] private float destroyVolume = 0.9f;

    [Header("Juice Config")]
    [SerializeField] private float bounceForce = 4f; 

    private Dictionary<Vector3Int, int> oreHealthTracker = new Dictionary<Vector3Int, int>();
    private Dictionary<ParticleSystem, MiningParticlePool> runtimeParticlePools = new Dictionary<ParticleSystem, MiningParticlePool>();
    private bool isWobbling = false; 

    void Start()
    {
        if (oreTilemap == null)
        {
            oreTilemap = GameObject.Find("OreTilemap")?.GetComponent<Tilemap>();
        }

        SetupFeedbackPools();

        // --------- 【优化修复 Bug 2：出生/刷新时单次排查卡死】 ---------
        ResolveInitialStuckPlayers();
    }

    private void SetupFeedbackPools()
    {
        ParticleSystem hitPrefab = fallbackHitParticlePrefab;
        if (hitPrefab == null)
        {
            hitPrefab = FindDefaultHitParticlePrefab();
        }

        if (hitPrefab != null)
        {
            hitParticlePool = EnsureParticlePool(hitParticlePool, "Hit Particle Pool", hitPrefab, hitParticleInitialPoolSize, hitParticleMaxPoolSize);
            runtimeParticlePools[hitPrefab] = hitParticlePool;
        }

        if (destroyParticlePrefab != null)
        {
            destroyParticlePool = EnsureParticlePool(destroyParticlePool, "Destroy Particle Pool", destroyParticlePrefab, destroyParticleInitialPoolSize, destroyParticleMaxPoolSize);
            runtimeParticlePools[destroyParticlePrefab] = destroyParticlePool;
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private ParticleSystem FindDefaultHitParticlePrefab()
    {
        if (oreTilemap == null)
        {
            return null;
        }

        foreach (Vector3Int position in oreTilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = oreTilemap.GetTile(position);
            if (tile is MiningTile ore && ore.hitParticlePrefab != null)
            {
                return ore.hitParticlePrefab;
            }
        }

        return null;
    }

    private MiningParticlePool EnsureParticlePool(MiningParticlePool pool, string poolName, ParticleSystem prefab, int initialPoolSize, int maxPoolSize)
    {
        if (pool == null)
        {
            GameObject poolObject = new GameObject(poolName);
            poolObject.transform.SetParent(transform);
            pool = poolObject.AddComponent<MiningParticlePool>();
        }

        pool.Configure(prefab, initialPoolSize, maxPoolSize);
        return pool;
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

        PlayHitFeedback(cellWorldPos, ore);

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
        Vector3 spawnPosition = oreTilemap.GetCellCenterWorld(gridPos);

        PlayDestroyFeedback(spawnPosition);

        oreTilemap.SetColor(gridPos, Color.white);
        oreTilemap.SetTileFlags(gridPos, TileFlags.LockColor);
        oreTilemap.SetTile(gridPos, null);

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

    private void PlayHitFeedback(Vector3 position, MiningTile ore)
    {
        ParticleSystem hitPrefab = ore.hitParticlePrefab != null ? ore.hitParticlePrefab : fallbackHitParticlePrefab;
        if (hitPrefab != null)
        {
            MiningParticlePool pool = GetPoolForPrefab(hitPrefab, "Hit Particle Pool", hitParticleInitialPoolSize, hitParticleMaxPoolSize);
            pool.Play(position, Quaternion.identity);
        }

        PlayOneShot(hitClip, hitVolume);
    }

    private void PlayDestroyFeedback(Vector3 position)
    {
        if (destroyParticlePrefab != null)
        {
            MiningParticlePool pool = GetPoolForPrefab(destroyParticlePrefab, "Destroy Particle Pool", destroyParticleInitialPoolSize, destroyParticleMaxPoolSize);
            pool.Play(position, Quaternion.identity);
        }

        PlayOneShot(destroyClip, destroyVolume);
    }

    private void PlayOneShot(AudioClip clip, float volume)
    {
        if (audioSource == null || clip == null)
        {
            return;
        }

        audioSource.PlayOneShot(clip, volume);
    }

    private MiningParticlePool GetPoolForPrefab(ParticleSystem prefab, string poolName, int initialPoolSize, int maxPoolSize)
    {
        if (runtimeParticlePools.TryGetValue(prefab, out MiningParticlePool pool) && pool != null)
        {
            return pool;
        }

        if (prefab == fallbackHitParticlePrefab && hitParticlePool != null)
        {
            pool = hitParticlePool;
        }
        else if (prefab == destroyParticlePrefab && destroyParticlePool != null)
        {
            pool = destroyParticlePool;
        }
        else
        {
            GameObject poolObject = new GameObject($"{poolName} ({prefab.name})");
            poolObject.transform.SetParent(transform);
            pool = poolObject.AddComponent<MiningParticlePool>();
        }

        pool.Configure(prefab, initialPoolSize, maxPoolSize);
        runtimeParticlePools[prefab] = pool;
        return pool;
    }
}
