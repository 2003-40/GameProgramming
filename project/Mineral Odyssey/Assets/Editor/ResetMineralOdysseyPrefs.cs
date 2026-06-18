using UnityEditor;
using UnityEngine;

public static class ResetMineralOdysseyPrefs
{
    [MenuItem("Tools/Mineral Odyssey/Reset Fresh Save")]
    public static void ResetFreshSave()
    {
        PlayerPrefs.DeleteKey("Gold");
        PlayerPrefs.DeleteKey("ToolUpgradeLevel");
        PlayerPrefs.DeleteKey("WeaponUpgradeLevel");
        PlayerPrefs.DeleteKey("FirstLevelTutorialComplete");
        PlayerPrefs.Save();

        Debug.Log("Mineral Odyssey fresh save reset: Gold, ToolUpgradeLevel, WeaponUpgradeLevel, FirstLevelTutorialComplete cleared.");
    }
}
