using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager instance;

    [Header("Inventory Config")]
    [SerializeField] private int maxSlots = 24;
    [SerializeField] private List<ItemStack> items = new List<ItemStack>();

    private ReadOnlyCollection<ItemStack> readonlyItems;

    public static InventoryManager Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            instance = FindFirstObjectByType<InventoryManager>();

            if (instance == null)
            {
                GameObject inventoryObject = new GameObject(nameof(InventoryManager));
                instance = inventoryObject.AddComponent<InventoryManager>();
            }

            return instance;
        }
    }

    public IReadOnlyList<ItemStack> Items => readonlyItems ?? (readonlyItems = items.AsReadOnly());
    public int MaxSlots => maxSlots;

    public event Action<IReadOnlyList<ItemStack>> InventoryChanged;
    public event Action<ItemData, int> ItemAdded;
    public event Action<ItemData, int> ItemRemoved;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        readonlyItems = items.AsReadOnly();
        DontDestroyOnLoad(gameObject);
    }

    public bool AddItem(ItemData item)
    {
        if (item == null)
        {
            return false;
        }

        ItemStack existingStack = FindStack(item);
        if (existingStack != null)
        {
            existingStack.Add(1);
            ItemAdded?.Invoke(item, existingStack.Quantity);
            NotifyInventoryChanged();
            return true;
        }

        if (IsFull())
        {
            return false;
        }

        ItemStack newStack = new ItemStack(item, 1);
        items.Add(newStack);
        ItemAdded?.Invoke(item, newStack.Quantity);
        NotifyInventoryChanged();
        return true;
    }

    public bool RemoveItem(ItemData item)
    {
        if (item == null)
        {
            return false;
        }

        ItemStack existingStack = FindStack(item);
        if (existingStack == null)
        {
            return false;
        }

        existingStack.Remove(1);
        int remainingQuantity = existingStack.Quantity;

        if (remainingQuantity <= 0)
        {
            items.Remove(existingStack);
        }

        ItemRemoved?.Invoke(item, Mathf.Max(remainingQuantity, 0));
        NotifyInventoryChanged();
        return true;
    }

    public bool IsFull()
    {
        return items.Count >= maxSlots;
    }

    private ItemStack FindStack(ItemData item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].Matches(item))
            {
                return items[i];
            }
        }

        return null;
    }

    private void NotifyInventoryChanged()
    {
        InventoryChanged?.Invoke(Items);
    }
}
