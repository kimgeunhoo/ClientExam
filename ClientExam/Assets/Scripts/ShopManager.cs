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

        bool added = inventoryManager.AddItem(item, amount);

        if (!added)
        {
            inventoryManager.AddGold(price);
            Debug.Log("인벤토리 공간 부족");
            return false;
        }

        return true;
    }

    public bool SellItem(ItemData item, int amount, int count = 1)
    {
        if (inventoryManager.GetItemCount(item) < amount)
            return false;

        int sellPrice = item.sellPrice * count;

        inventoryManager.RemoveItem(item, amount);
        inventoryManager.AddGold(sellPrice);

        return true;
    }
}
