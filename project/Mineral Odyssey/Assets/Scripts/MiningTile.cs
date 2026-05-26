using UnityEngine;
using UnityEngine.Tilemaps;

// 允许你在 Project 窗口右键创建这个资产
[CreateAssetMenu(fileName = "New Mining Tile", menuName = "Tiles/Mining Tile")]
public class MiningTile : Tile
{
    [Header("Mining Settings")]
    public string gemstoneName;      // 宝石名称 (例如 "Blue Gemstone")
    public int maxHealth = 3;         // 需要挖掘的次数
    public GameObject dropPrefab;    // 挖掘成功后掉落的物理物体预制体
}