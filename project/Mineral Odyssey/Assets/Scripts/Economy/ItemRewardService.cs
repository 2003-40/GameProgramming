using UnityEngine;

public static class ItemRewardService
{
    public static bool Grant(ItemData itemData, ItemRewardMode rewardMode)
    {
        if (itemData == null)
        {
            return false;
        }

        switch (rewardMode)
        {
            case ItemRewardMode.AutoConvertToGold:
                string itemName = string.IsNullOrWhiteSpace(itemData.DisplayName) ? itemData.name : itemData.DisplayName;
                int updatedGold = GoldManager.Instance.AddGold(itemData.Value);
                Debug.Log($"[Gold Reward] Collected Item: {itemName} | Item Value: {itemData.Value} | Updated Gold Total: {updatedGold}");
                return true;

            case ItemRewardMode.AddToInventory:
                return InventoryManager.Instance.AddItem(itemData);

            default:
                Debug.LogWarning($"Unsupported reward mode {rewardMode} for {itemData.DisplayName}.");
                return false;
        }
    }
}
