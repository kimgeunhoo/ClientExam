using UnityEngine;

[System.Serializable]
public class InventorySlotData
{
    public ItemData item;
    public int count;

    public bool IsEmpty => item == null || count <= 0;

    public void SetItem(ItemData newItem, int amount)
    {
        item = newItem;
        count = amount;
    }

    public void Clear()
    {
        item = null;
        count = 0;
    }

}
