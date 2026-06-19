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

        if (currentMerchant == null)
            return;

        foreach (ShopItemData shopItem in currentMerchant.SellItems)
        {
            ShopSlotUI slot = Instantiate(buySlotPrefab, buyContentParent);

            slot.Init(
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

    private void OnBuyClicked(ItemData item, int amount, int price)
    {
        bool success = shopManager.BuyItem(item, amount, price);

        if (success)
            RefreshAll();
    }

    private void OnSellClicked(ItemData item, int amount, int price)
    {
        bool success = shopManager.SellItem(item, amount);

        if (success)
            RefreshAll();
    }
    private void OnSellClicked(ItemData item, int count, int price, RectTransform buttonRect)
    {
        if (count >= popupThreshold)
        {
            if (sellAmountPopup == null)
            {
                Debug.LogError("SellAmountPopup이 ShopUI에 연결되지 않았습니다.");
                return;
            }

            sellAmountPopup.Show(
                item,
                count,
                price,
                buttonRect,
                SellSelectedAmount);

            return;
        }

        SellSelectedAmount(item, 1, price);
    }
    private void SellSelectedAmount(ItemData item, int amount, int price)
{
    bool success = shopManager.SellItem(item, amount, price);

    if (success)
        RefreshAll();
}
    private void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }

}
