using System;
using UnityEngine;

/// <summary>
/// Serializable inventory entry that stores one item definition and its stack quantity.
/// </summary>
[Serializable]
public class ItemStack
{
    [SerializeField] private ItemData item;
    [SerializeField] private int quantity;

    public ItemData Item => item;
    public int Quantity => quantity;

    public ItemStack(ItemData item, int quantity)
    {
        this.item = item;
        this.quantity = Mathf.Max(0, quantity);
    }

    public bool Matches(ItemData other)
    {
        // Compare by object reference first, then by stable item id for copied ScriptableObjects.
        if (item == null || other == null)
        {
            return false;
        }

        return item == other || item.Id == other.Id;
    }

    public void Add(int amount)
    {
        quantity = Mathf.Max(0, quantity + amount);
    }

    public void Remove(int amount)
    {
        quantity = Mathf.Max(0, quantity - amount);
    }
}
