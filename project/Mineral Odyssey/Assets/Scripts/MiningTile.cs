using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Mining Tile", menuName = "Tiles/Mining Tile")]
public class MiningTile : Tile
{
    [Header("Mining Settings")]
    public string gemstoneName;      // 宝石名称
    public int maxHealth = 3;         // 需要挖掘的次数
    public GameObject dropPrefab;    // 挖掘成功后掉落的物理物体预制体

    [Header("UX & Mechanics (New)")]
    public int requiredToolLevel = 1;  // 3.2 挖掘工具等级限制
    public ParticleSystem hitParticlePrefab; // 2.2 受击粒子碎屑飞溅
    
    // 3.1 裂纹视觉变化：可以准备一组不同损坏程度的 Sprites（例如 0=轻微裂, 1=严重裂）
    // 或者我们直接在控制器里通过动态改变瓦片颜色（Color）变暗来表现，这里留出 Sprite 扩展
    public Sprite[] crackSprites; 
}