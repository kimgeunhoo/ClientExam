using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private Transform weaponSocket;

    private Dictionary<EquipSlotType, ItemData> equippedItems = new();
    private GameObject currentWeaponObject;

    public bool Equip(ItemData newItem, out ItemData oldItem)
    {
        oldItem = null;

        if (newItem == null || newItem.itemType != ItemType.Equipment)
            return false;

        EquipSlotType slotType = newItem.equipSlotType;

        if (equippedItems.TryGetValue(slotType, out oldItem))
        {
            RemoveStats(oldItem);
        }

        equippedItems[slotType] = newItem;
        AddStats(newItem);
        if (slotType == EquipSlotType.Weapon)
        {
            ShowWeaponModel(newItem);
        }

        Debug.Log($"ÀåÂø ¿Ï·á: {newItem.itemName}");
        return true;
    }
    public void Unequip(EquipSlotType slotType)
    {
        if (!equippedItems.TryGetValue(slotType, out ItemData item))
            return;

        RemoveStats(item);

        equippedItems.Remove(slotType);

        if (slotType == EquipSlotType.Weapon)
        {
            ShowWeaponModel(null);
        }
    }
    public ItemData GetEquippedItem(EquipSlotType slotType)
    {
        if (equippedItems.TryGetValue(slotType, out ItemData item))
            return item;

        return null;
    }

    private void ShowWeaponModel(ItemData item)
    {
        if (currentWeaponObject != null)
        {
            Destroy(currentWeaponObject);
            currentWeaponObject = null;
        }

        if (item == null || item.equipPrefab == null)
            return;

        currentWeaponObject = Instantiate(item.equipPrefab, weaponSocket);

        currentWeaponObject.transform.localPosition = Vector3.zero;
        currentWeaponObject.transform.localRotation = Quaternion.identity;
        currentWeaponObject.transform.localScale = Vector3.one;
    }

    public bool HasPickaxe()
    {
        ItemData tool = GetEquippedItem(EquipSlotType.Weapon);

        return tool != null && tool.toolType == ToolType.Pickaxe;
    }
    private void AddStats(ItemData item)
    {
        if (characterStats == null || item.statModifiers == null)
            return;

        foreach (StatModifierData modifier in item.statModifiers)
            characterStats.AddModifier(modifier);
    }

    private void RemoveStats(ItemData item)
    {
        if (characterStats == null || item.statModifiers == null)
            return;

        foreach (StatModifierData modifier in item.statModifiers)
            characterStats.RemoveModifier(modifier);
    }
}
