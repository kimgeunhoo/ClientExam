using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;

    public bool BuyItem(ItemData item, int amount, int price)
    {
        if (!inventoryManager.SpendGold(price))
        {
            Debug.Log("골드 부족");
            return false;
        }

        int totalPrice = price * amount;

        bool added = inventoryManager.AddItem(item, amount);
       
        if (!added)
        {
            inventoryManager.AddGold(price);
            Debug.Log("인벤토리 공간 부족");
            return false;
        }

        return true;
    }

    public bool SellItemFromSlot(int slotIndex, int amount, int sellPrice)
    {
        if (inventoryManager == null)
            return false;

        bool removed = inventoryManager.RemoveItemAt(slotIndex, amount);

        if (!removed)
            return false;

        inventoryManager.AddGold(sellPrice * amount);

        return true;
    }
}
