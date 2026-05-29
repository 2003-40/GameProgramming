using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData itemData;

    public ItemData ItemData => itemData;

    public bool TryCollect()
    {
        if (itemData == null)
        {
            Debug.LogWarning($"ItemPickup on {name} is missing ItemData.");
            return false;
        }

        if (!InventoryManager.Instance.AddItem(itemData))
        {
            return false;
        }

        Destroy(gameObject);
        return true;
    }
}
