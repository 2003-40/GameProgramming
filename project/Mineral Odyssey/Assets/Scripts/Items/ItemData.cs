using UnityEngine;

/// <summary>
/// ScriptableObject definition for items, including display data and gold conversion value.
/// </summary>
[CreateAssetMenu(fileName = "NewItemData", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string itemId;
    [SerializeField] private string displayName;

    [Header("Presentation")]
    [SerializeField] private Sprite icon;
    [SerializeField] private ItemType itemType = ItemType.Ore;

    [Header("Economy")]
    [SerializeField] private int value = 1;

    public string Id => itemId;
    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public ItemType Type => itemType;
    public int Value => value;
}
