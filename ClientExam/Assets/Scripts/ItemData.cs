using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Item Info")]
    public string itemName;
    public Sprite icon;
    public int maxStack = 1;
    public ItemType itemType;

    [Header("Equipment")]
    public EquipSlotType equipSlotType;
    public StatModifierData[] statModifiers;
}
