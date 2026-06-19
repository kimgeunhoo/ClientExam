using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button actionButton;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private TextMeshProUGUI amountText;

    private int inventorySlotIndex;
    private ItemData itemData;
    private int amount;
    private int price;
    private System.Action<ItemData, int, int> onClick;

    public void Init(
       int slotIndex,
       ItemData item,
       int amount,
       int price,
       string buttonLabel,
       System.Action<ItemData, int, int> clickAction)
    {
        inventorySlotIndex = slotIndex;
        itemData = item;
        this.amount = amount;
        this.price = price;
        onClick = clickAction;

        iconImage.sprite = item.icon;
        iconImage.enabled = item.icon != null;

        priceText.text = $"Price : {price}";
        buttonText.text = "Sold";

        if (amountText != null)
            amountText.text = amount > 1 ? $"x{amount}" : "";

        RectTransform buttonRect = actionButton.GetComponent<RectTransform>();

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() =>
        {
            onClick?.Invoke(inventorySlotIndex, itemData, amount, this.price, buttonRect);
        });
    }

    public void InitSell(
    ItemData item,
    int count,
    int price,
    string buttonLabel,
    System.Action<ItemData, int, int, RectTransform> clickAction)
    {
        itemData = item;
        amount = count;
        this.price = price;

        iconImage.sprite = item.icon;
        iconImage.enabled = item.icon != null;

        priceText.text = $"Price : {price}";
        buttonText.text = buttonLabel;

        if (amountText != null)
            amountText.text = count > 1 ? $"x{count}" : "";

        RectTransform buttonRect = actionButton.GetComponent<RectTransform>();

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() =>
        {
            clickAction?.Invoke(itemData, amount, this.price, buttonRect);
        });
    }
}
