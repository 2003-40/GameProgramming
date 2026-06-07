using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private ItemRewardMode rewardMode = ItemRewardMode.AutoConvertToGold;

    public ItemData ItemData => itemData;
    public ItemRewardMode RewardMode => rewardMode;

    public bool TryCollect()
    {
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
            RunCardManager.Instance.RegisterCardTimerStartAction();
        }

        Destroy(gameObject);
        return true;
    }
}
