using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OreGenerator : MonoBehaviour
{
    [System.Serializable]
    public class OreSpawnData
    {
        public string oreName;       // Ore name
        public TileBase oreTile;     // Tile asset for this ore
        [Range(0, 100)]
        public float spawnWeight;    // Relative weight after a cell has been selected for ore spawning
    }

    [Header("References")]
    [SerializeField] private Tilemap oreTilemap;

    [Header("Generation Bounds (Grid Coordinates)")]
    [SerializeField] private int minX = -19;
    [SerializeField] private int maxX = 9;
    [SerializeField] private int minY = -2;
    [SerializeField] private int maxY = 4;

    [Header("Global Spawn Density (0-100%)")]
    [Tooltip("Chance for each cell to spawn ore. Empty cells stay blank so the background remains visible.")]
    [Range(0f, 100f)] [SerializeField] private float globalSpawnChance = 15f;

    [Header("Ore Types And Weights")]
    [SerializeField] private List<OreSpawnData> orePool = new List<OreSpawnData>();

    void Start()
    {
        GenerateMine();
    }

    [ContextMenu("Regenerate Mine")]
    public void GenerateMine()
    {
        oreTilemap.ClearAllTiles();

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                // First decide whether this cell should spawn ore.
                if (Random.Range(0f, 100f) <= globalSpawnChance)
                {
                    Vector3Int currentPos = new Vector3Int(x, y, 0);
                    
                    // Then choose which ore type to place.
                    TileBase tileToPlace = GetRandomOreFromPool();

                    if (tileToPlace != null)
                    {
                        oreTilemap.SetTile(currentPos, tileToPlace);
                    }
                }
                // If the roll misses, leave this Tilemap cell empty.
            }
        }

        // Refresh Tilemap rendering and physics after generation.
        oreTilemap.RefreshAllTiles();

        // Rebuild CompositeCollider2D geometry at runtime if it is attached.
        if (oreTilemap.TryGetComponent<CompositeCollider2D>(out var compositeCollider))
        {
            compositeCollider.GenerateGeometry();
            Debug.Log("OreGenerator: Physics geometry rebuilt; ore walls are active.");
        }
    }

    private TileBase GetRandomOreFromPool()
    {
        if (orePool == null || orePool.Count == 0) return null;

        // Calculate total weight.
        float totalWeight = 0f;
        foreach (var ore in orePool)
        {
            totalWeight += ore.spawnWeight;
        }

        // Roll against the weighted ore pool.
        float roll = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        foreach (var ore in orePool)
        {
            cumulativeWeight += ore.spawnWeight;
            if (roll <= cumulativeWeight)
            {
                return ore.oreTile;
            }
        }

        return null;
    }
}
