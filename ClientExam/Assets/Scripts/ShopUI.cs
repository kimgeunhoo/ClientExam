using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject shopPanel;

    [Header("Buy")]
    [SerializeField] private Transform buyContentParent;
    [SerializeField] private ShopSlotUI buySlotPrefab;

    [Header("Sell")]
    [SerializeField] private Transform sellContentParent;
    [SerializeField] private ShopSlotUI sellSlotPrefab;

    [Header("Gold")]
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("Refs")]
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private InventoryManager inventoryManager;

    [Header("SellUI")]
    [SerializeField] private SellAmountPopup sellAmountPopup;
    [SerializeField] private int popupThreshold = 2;

    private Merchant currentMerchant;

    private void Awake()
    {
        shopPanel.SetActive(false);
    }

    public void Open(Merchant merchant)
    {
        currentMerchant = merchant;
        shopPanel.SetActive(true);

        RefreshAll();
    }

    public void Close()
    {
        currentMerchant = null;
        shopPanel.SetActive(false);
    }

    private void RefreshAll()
    {
        RefreshGold();
        RefreshBuyList();
        RefreshSellList();
    }

    private void RefreshGold()
    {
        goldText.text = inventoryManager.Gold.ToString();
    }

    private void RefreshBuyList()
    {
        ClearChildren(buyContentParent);

        if (currentMerchant == null || currentMerchant.SellItems == null)
            return;

        ShopItemData[] sellItems = currentMerchant.SellItems;

        for (int i = 0; i < sellItems.Length; i++)
        {
            ShopItemData shopItem = sellItems[i];

            ShopSlotUI slot = Instantiate(buySlotPrefab, buyContentParent);

            slot.InitBuy(
                i,
                shopItem.itemData,
                shopItem.amount,
                shopItem.price,
                "Buy",
                OnBuyClicked);
        }
    }

    private void RefreshSellList()
    {
        ClearChildren(sellContentParent);

        List<InventorySlotData> slots = inventoryManager.GetSlots();

        for (int i = 0; i < slots.Count; i++)
        {
            InventorySlotData slotData = slots[i];

            if (slotData == null || slotData.IsEmpty)
                continue;

            ItemData item = slotData.item;
            int count = slotData.count;
            int unitPrice = item.sellPrice;

            ShopSlotUI slot = Instantiate(sellSlotPrefab, sellContentParent);

            slot.InitSell(
                i,
                item,
                count,
                unitPrice,
                "Sold",
                OnSellClicked);
        }
    }

    private void OnBuyClicked(int shopItemIndex, ItemData item, int amount, int price)
    {
        bool success = shopManager.BuyItem(item, amount, price);

        if (success)
            RefreshAll();
    }

    private void OnSellClicked(int slotIndex, ItemData item, int amount, int price, RectTransform buttonRect)
    {
        if (amount > 1)
        {
            sellAmountPopup.Show(slotIndex, item, amount, price, buttonRect, SellSelectedAmount);

            return;
        }

        SellSelectedAmount(slotIndex, item, 1, price);
    }

    private void SellSelectedAmount(int slotIndex, ItemData item, int amount, int price)
{
    bool success = shopManager.SellItemFromSlot(slotIndex, amount, price);

    if (success)
        RefreshAll();
}
    private void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }

}
