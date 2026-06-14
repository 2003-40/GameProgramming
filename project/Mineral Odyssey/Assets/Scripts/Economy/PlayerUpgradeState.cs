using System;
using UnityEngine;

/// <summary>
/// Stores permanent tool and weapon upgrade progression using PlayerPrefs.
/// </summary>
public static class PlayerUpgradeState
{
    private const string ToolLevelKey = "ToolUpgradeLevel";
    private const string WeaponLevelKey = "WeaponUpgradeLevel";

    private static readonly int[] ToolUpgradeCosts = { 75, 150 };
    private static readonly int[] WeaponUpgradeCosts = { 50, 100 };

    public const int MinToolLevel = 1;
    public const int MaxToolLevel = 3;
    public const int MinWeaponLevel = 1;
    public const int MaxWeaponLevel = 3;

    public static event Action UpgradesChanged;

    public static int ToolLevel => Mathf.Clamp(PlayerPrefs.GetInt(ToolLevelKey, MinToolLevel), MinToolLevel, MaxToolLevel);
    public static int WeaponLevel => Mathf.Clamp(PlayerPrefs.GetInt(WeaponLevelKey, MinWeaponLevel), MinWeaponLevel, MaxWeaponLevel);

    public static bool IsToolMaxed => ToolLevel >= MaxToolLevel;
    public static bool IsWeaponMaxed => WeaponLevel >= MaxWeaponLevel;

    public static int ToolMiningPower => ToolLevel;
    public static int WeaponDamage => WeaponLevel;
    public static string WeaponTierName => GetWeaponTierName(WeaponLevel);

    public static float ToolStaminaMultiplier => 1f + ((ToolLevel - MinToolLevel) * 0.25f);

    public static int ToolUpgradeCost => GetUpgradeCost(ToolLevel, ToolUpgradeCosts);
    public static int WeaponUpgradeCost => GetUpgradeCost(WeaponLevel, WeaponUpgradeCosts);

    public static bool TryUpgradeTool(out int cost)
    {
        // Tool progression is intentionally short so the vertical slice reaches harder ore quickly.
        return TryUpgrade(ToolLevelKey, ToolLevel, MaxToolLevel, ToolUpgradeCosts, out cost);
    }

    public static bool TryUpgradeWeapon(out int cost)
    {
        return TryUpgrade(WeaponLevelKey, WeaponLevel, MaxWeaponLevel, WeaponUpgradeCosts, out cost);
    }

    public static int ReduceMonsterStaminaDamage(int baseDamage)
    {
        // Weapon upgrades reduce contact punishment but always leave at least one stamina damage.
        if (baseDamage <= 0)
        {
            return 0;
        }

        int reduction = WeaponLevel - MinWeaponLevel;
        return Mathf.Max(1, baseDamage - reduction);
    }

    public static string GetWeaponTierName(int weaponLevel)
    {
        switch (Mathf.Clamp(weaponLevel, MinWeaponLevel, MaxWeaponLevel))
        {
            case 1:
                return "Copper";
            case 2:
                return "Iron";
            case 3:
                return "Crystal";
            default:
                return "Copper";
        }
    }

    private static bool TryUpgrade(string key, int currentLevel, int maxLevel, int[] costs, out int cost)
    {
        cost = GetUpgradeCost(currentLevel, costs);
        if (currentLevel >= maxLevel)
        {
            return false;
        }

        PlayerPrefs.SetInt(key, currentLevel + 1);
        PlayerPrefs.Save();
        UpgradesChanged?.Invoke();
        return true;
    }

    private static int GetUpgradeCost(int currentLevel, int[] costs)
    {
        int costIndex = currentLevel - 1;
        if (costIndex < 0 || costIndex >= costs.Length)
        {
            return 0;
        }

        return costs[costIndex];
    }
}
