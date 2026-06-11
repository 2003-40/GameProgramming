using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Central mining system that validates tool gates, spends stamina, damages ore tiles, and spawns rewards.
/// </summary>
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

        // Resolve one-time spawn overlap after the ore map is generated or refreshed.
        ResolveInitialStuckPlayers();
    }

    private void SetupFeedbackPools()
    {
        // Feedback is pooled so rapid mining does not repeatedly instantiate particles.
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
    /// Runs a one-time check to prevent the player from starting inside generated ore.
    /// </summary>
    private void ResolveInitialStuckPlayers()
    {
        Player player = FindFirstObjectByType<Player>();
        if (player == null || oreTilemap == null) return;

        Collider2D playerCol = player.GetComponent<Collider2D>();
        if (playerCol == null) return;

        // Get the player's current center cell.
        Vector3Int startGrid = oreTilemap.WorldToCell(player.transform.position);
        
        // Check nearby cells in a 3x3 range.
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int checkPos = startGrid + new Vector3Int(x, y, 0);
                if (oreTilemap.HasTile(checkPos))
                {
                    Vector3 cellCenter = oreTilemap.GetCellCenterWorld(checkPos);
                    Bounds tileBounds = new Bounds(cellCenter, oreTilemap.cellSize);

                    // Clear any ore tile overlapping the player at spawn.
                    if (playerCol.bounds.Intersects(tileBounds))
                    {
                        Debug.LogWarning($"[Safe Startup] Player spawned inside ore at {checkPos}; clearing the tile once.");
                        oreTilemap.SetTile(checkPos, null); // Remove the hazardous ore tile.
                    }
                }
            }
        }
    }

    /// <summary>
    /// Called directly by tool scripts after they detect a hit.
    /// </summary>
    /// <param name="worldHitPos">World-space point hit by the swing.</param>
    /// <param name="incomingToolLevel">Current level of the equipped player tool.</param>
    public void TryMineAtPosition(Vector3 worldHitPos, int incomingToolLevel, float toolEfficiency)
    {
        // Convert the animation/tool hit point into the matching tilemap cell.
        if (oreTilemap == null) return;

        Vector3Int gridPos = oreTilemap.WorldToCell(worldHitPos);
        TileBase clickedTile = oreTilemap.GetTile(gridPos);

        if (clickedTile is MiningTile currentOre)
        {
            // Validate tool level.
            if (incomingToolLevel < currentOre.requiredToolLevel)
            {
                Debug.Log($"[Mining Blocked] ToolLevel={incomingToolLevel}, RequiredToolLevel={currentOre.requiredToolLevel}, Ore={currentOre.gemstoneName}, OreHardness={currentOre.hardness}");
                return;
            }

            bool shouldConsumeStamina = RunCardManager.Instance.ShouldConsumeStaminaForMiningHit();
            int staminaCost = shouldConsumeStamina
                ? RunCardManager.Instance.ModifyStaminaCost(CalculateStaminaCost(currentOre, incomingToolLevel, toolEfficiency))
                : 0;
            Debug.Log($"[Mining] ToolLevel={incomingToolLevel}, Ore={currentOre.gemstoneName}, RequiredToolLevel={currentOre.requiredToolLevel}, OreHardness={currentOre.hardness}, OreStaminaMultiplier={currentOre.staminaCostMultiplier}, ToolEfficiency={toolEfficiency}, StaminaCost={staminaCost}");
            if (shouldConsumeStamina && !StaminaManager.Instance.ConsumeStamina(staminaCost))
            {
                Debug.Log("[Insufficient Stamina] Mining stopped; the current exploration is over.");
                return;
            }

            RunCardManager.Instance.RegisterSuccessfulMiningAction();
            if (RunCardManager.Instance.ShouldMiningHitFail())
            {
                Vector3 failedHitWorldPos = oreTilemap.GetCellCenterWorld(gridPos);
                PlayHitFeedback(failedHitWorldPos, currentOre);
                Debug.Log("[Run Card] Mining hit failed to damage the tile.");
                return;
            }

            HandleDamage(gridPos, currentOre);
        }
    }

    private int CalculateStaminaCost(MiningTile ore, int toolLevel, float toolEfficiency)
    {
        // Harder ores cost more stamina, while better tools reduce the cost.
        int hardness = Mathf.Max(1, ore.hardness);
        float oreMultiplier = Mathf.Max(0.01f, ore.staminaCostMultiplier);
        int miningPower = Mathf.Max(1, toolLevel);
        float efficiency = Mathf.Max(0.01f, toolEfficiency);
        return Mathf.Max(1, Mathf.CeilToInt((hardness * oreMultiplier) / (miningPower * efficiency)));
    }

    private void HandleDamage(Vector3Int gridPos, MiningTile ore)
    {
        // Ore health is tracked per cell because the same Tile asset can appear many times.
        if (!oreHealthTracker.ContainsKey(gridPos))
        {
            oreHealthTracker.Add(gridPos, ore.maxHealth);
        }

        oreHealthTracker[gridPos]--;
        Vector3 cellWorldPos = oreTilemap.GetCellCenterWorld(gridPos);

        PlayHitFeedback(cellWorldPos, ore);

        // Shake the ore tilemap when an ore tile is hit.
        if (!isWobbling)
        {
            StartCoroutine(WobbleTilemapVisual());
        }

        // Multi-stage damage tinting.
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
        // Reset tile visual state before removing it so regenerated or reused cells are clean.
        Vector3 spawnPosition = oreTilemap.GetCellCenterWorld(gridPos);

        PlayDestroyFeedback(spawnPosition);

        oreTilemap.SetColor(gridPos, Color.white);
        oreTilemap.SetTileFlags(gridPos, TileFlags.LockColor);
        oreTilemap.SetTile(gridPos, null);

        if (ore.dropPrefab != null)
        {
            int dropCount = RunCardManager.Instance.GetOreDropCount();
            for (int i = 0; i < dropCount; i++)
            {
                SpawnDrop(ore.dropPrefab, spawnPosition);
            }
        }

        oreHealthTracker.Remove(gridPos);
    }

    private void SpawnDrop(GameObject dropPrefab, Vector3 spawnPosition)
    {
        GameObject droppedItem = Instantiate(dropPrefab, spawnPosition, Quaternion.identity);
        Rigidbody2D rb = droppedItem.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            rb.AddForce(randomDirection * bounceForce, ForceMode2D.Impulse);
        }
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
