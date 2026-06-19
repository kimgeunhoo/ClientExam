using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Item Info")]
    public string itemName;
    public Sprite icon;
    public int maxStack = 1;
    public ItemType itemType;

    [Header("Price")]
    public int buyPrice = 100;
    public int sellPrice = 50;

    [Header("Tool")]
    public ToolType toolType;

    [Header("Equipment")]
    public EquipSlotType equipSlotType;
    public StatModifierData[] statModifiers;
    public GameObject equipPrefab;
}
