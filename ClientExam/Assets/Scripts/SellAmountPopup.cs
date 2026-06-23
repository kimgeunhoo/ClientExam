using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellAmountPopup : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Slider amountSlider;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private ItemData currentItem;
    private int currentPrice;
    private System.Action<int, ItemData, int, int> onConfirm;
    private int currentSlotIndex;

    private void Awake()
    {
        root.SetActive(false);

        amountSlider.onValueChanged.AddListener(OnSliderChanged);
        cancelButton.onClick.AddListener(Hide);
    }
    public void Show(
        int slotIndex,
        ItemData item,
        int maxAmount,
        int price,
        RectTransform targetButton,
        System.Action<int, ItemData, int, int> confirmAction)
    {
        currentSlotIndex = slotIndex;
        currentItem = item;
        currentPrice = price;
        onConfirm = confirmAction;

        root.SetActive(true);
        root.transform.SetAsLastSibling();

        titleText.text = "몇 개를 판매하시겠습니까?";

        amountSlider.minValue = 1;
        amountSlider.maxValue = maxAmount;
        amountSlider.wholeNumbers = true;
        amountSlider.value = 1;

        UpdateAmountText();

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(Confirm);

        MoveNearButton(targetButton);
    }

    private void OnSliderChanged(float value)
    {
        UpdateAmountText();
    }

    private void UpdateAmountText()
    {
        int amount = Mathf.RoundToInt(amountSlider.value);
        amountText.text = amount.ToString();
    }

    private void Confirm()
    {
        int amount = Mathf.RoundToInt(amountSlider.value);
        onConfirm?.Invoke(currentSlotIndex, currentItem, amount, currentPrice);
        Hide();
    }

    public void Hide()
    {
        root.SetActive(false);
    }

    private void MoveNearButton(RectTransform targetButton)
    {
        RectTransform rootRect = root.GetComponent<RectTransform>();

        Vector3[] corners = new Vector3[4];
        targetButton.GetWorldCorners(corners);

        Vector3 buttonCenter = (corners[0] + corners[2]) * 0.5f;
        rootRect.position = buttonCenter + new Vector3(0f, 120f, 0f);
    }

}
