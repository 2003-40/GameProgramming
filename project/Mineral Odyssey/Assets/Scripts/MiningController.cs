using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MiningController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap oreTilemap;    // 将你的 OreTilemap 拖到这里
    [SerializeField] private Camera mainCamera;    // 拖入主相机

    [Header("Mining Config")]
    [SerializeField] private float miningRange = 1.5f; // 玩家能挖到的最大距离

    // 用字典记录地图上哪些坐标的矿石被消了多少血
    private Dictionary<Vector3Int, int> oreHealthTracker = new Dictionary<Vector3Int, int>();

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        
        // 自动抓取组件兜底
        if (oreTilemap == null)
        {
            oreTilemap = GameObject.Find("OreTilemap")?.GetComponent<Tilemap>();
        }
    }

    void Update()
    {
        // 当玩家按下鼠标左键（或你设定的攻击键）
        if (Input.GetMouseButtonDown(0))
        {
            ProcessMining();
        }
    }

    private void ProcessMining()
    {
        // 1. 获取鼠标在世界空间中的位置
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        // 2. 距离检查：计算玩家与点击的目标矿石之间的物理距离
        float distance = Vector3.Distance(transform.position, mouseWorldPos);
        if (distance > miningRange)
        {
            Debug.Log("太远了，手不够长！");
            return;
        }

        // 3. 将世界坐标转换为 Tilemap 上的网格坐标 (Grid Cell Position)
        Vector3Int gridPos = oreTilemap.WorldToCell(mouseWorldPos);

        // 4. 尝试获取该格子上的瓦片
        TileBase clickedTile = oreTilemap.GetTile(gridPos);

        // 5. 核心判定：检查这个瓦片是不是我们自定义的 MiningTile
        if (clickedTile is MiningTile currentOre)
        {
            HandleDamage(gridPos, currentOre);
        }
    }

    private void HandleDamage(Vector3Int gridPos, MiningTile ore)
    {
        // 如果是第一次挖这个位置的矿，先初始化它的血量
        if (!oreHealthTracker.ContainsKey(gridPos))
        {
            oreHealthTracker.Add(gridPos, ore.maxHealth);
        }

        // 扣除 1 点生命值
        oreHealthTracker[gridPos]--;
        Debug.Log($"成功击中 [{ore.gemstoneName}]！剩余生命值: {oreHealthTracker[gridPos]}");

        // 检查矿石是否碎裂
        if (oreHealthTracker[gridPos] <= 0)
        {
            ExecuteDestruction(gridPos, ore);
        }
    }

    private void ExecuteDestruction(Vector3Int gridPos, MiningTile ore)
    {
        // 1. 将该位置的矿石瓦片清空 (核心技术点)
        oreTilemap.SetTile(gridPos, null);

        // 2. 计算该网格正中心的世界坐标，让掉落物生成在格子中间
        Vector3 spawnPosition = oreTilemap.GetCellCenterWorld(gridPos);

        // 3. 实例化掉落物
        if (ore.dropPrefab != null)
        {
            Instantiate(ore.dropPrefab, spawnPosition, Quaternion.identity);
            Debug.Log($"[{ore.gemstoneName}] 已破碎，掉落了资源！");
        }
        else
        {
            Debug.LogWarning($"[{ore.gemstoneName}] 破碎了，但你没有为它配置掉落物 Prefab！");
        }

        // 4. 从内存追踪中移除该坐标
        oreHealthTracker.Remove(gridPos);
    }
}