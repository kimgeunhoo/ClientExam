using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private int amount = 1;

    public ItemData ItemData => itemData;
    public int Amount => amount;

    public bool PickUp(InventoryManager inventory)
    {
        if (inventory == null || itemData == null)
            return false;

        bool success = inventory.AddItem(itemData, amount);

        if (success)
            Destroy(gameObject);

        return success;
    }
}
