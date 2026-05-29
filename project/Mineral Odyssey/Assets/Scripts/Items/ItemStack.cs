using System;
using UnityEngine;

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
