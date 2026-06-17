using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;

    public void SetSlot(InventorySlotData slotData)
    {
        if (slotData == null || slotData.IsEmpty)
        {
            iconImage.enabled = false;
            countText.text = "";
            return;
        }

        iconImage.enabled = true;
        iconImage.sprite = slotData.item.icon;

        countText.text = slotData.count > 1 ? slotData.count.ToString() : "";
    }
}
