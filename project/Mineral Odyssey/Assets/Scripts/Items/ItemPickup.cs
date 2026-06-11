using UnityEngine;

/// <summary>
/// Collectable world item that grants its configured reward and removes itself after collection.
/// </summary>
public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private ItemRewardMode rewardMode = ItemRewardMode.AutoConvertToGold;

    public ItemData ItemData => itemData;
    public ItemRewardMode RewardMode => rewardMode;

    public bool TryCollect()
    {
        // Missing ItemData is treated as a setup error instead of silently deleting the pickup.
        if (itemData == null)
        {
            Debug.LogWarning($"ItemPickup on {name} is missing ItemData.");
            return false;
        }

        if (!ItemRewardService.Grant(itemData, rewardMode))
        {
            return false;
        }

        if (itemData.Type == ItemType.Ore)
        {
            // Ore collection counts as active run progress for the card timer.
            RunCardManager.Instance.RegisterCardTimerStartAction();
        }

        Destroy(gameObject);
        return true;
    }
}
