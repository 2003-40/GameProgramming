using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OreGenerator : MonoBehaviour
{
    [System.Serializable]
    public class OreSpawnData
    {
        public string oreName;       // 矿石名称
        public TileBase oreTile;     // 对应的 Tile 资源
        [Range(0, 100)]
        public float spawnWeight;    // 在“已决定生成矿石”的格子中，该矿石占 know 多少权重
    }

    [Header("组件引用")]
    [SerializeField] private Tilemap oreTilemap;

    [Header("地图生成范围 (网格坐标)")]
    [SerializeField] private int minX = -10;
    [SerializeField] private int maxX = 10;
    [SerializeField] private int minY = -10;
    [SerializeField] private int maxY = 10;

    [Header("整体生成密度 (0-100%)")]
    [Tooltip("每个格子有多少概率会生成矿石。剩下的格子会保持空白，露出背景。")]
    [Range(0f, 100f)] [SerializeField] private float globalSpawnChance = 15f;

    [Header("矿石种类及权重配置")]
    [SerializeField] private List<OreSpawnData> orePool = new List<OreSpawnData>();

    void Start()
    {
        GenerateMine();
    }

    [ContextMenu("重新生成矿井")]
    public void GenerateMine()
    {
        oreTilemap.ClearAllTiles();

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                // 1. 首先独立判定：这个格子要不要长矿石？
                if (Random.Range(0f, 100f) <= globalSpawnChance)
                {
                    Vector3Int currentPos = new Vector3Int(x, y, 0);
                    
                    // 2. 如果要长，再决定长哪一种矿石
                    TileBase tileToPlace = GetRandomOreFromPool();

                    if (tileToPlace != null)
                    {
                        oreTilemap.SetTile(currentPos, tileToPlace);
                    }
                }
                // 如果没抽中，就什么都不做，这个位置在 Tilemap 上就是空的（透明的）
            }
        }
    }

    private TileBase GetRandomOreFromPool()
    {
        if (orePool == null || orePool.Count == 0) return null;

        // 计算总权重
        float totalWeight = 0f;
        foreach (var ore in orePool)
        {
            totalWeight += ore.spawnWeight;
        }

        // 投骰子
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