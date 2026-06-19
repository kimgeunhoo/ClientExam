using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemIconUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;

    private InventoryManager inventoryManager;
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Camera uiCamera;

    private int slotIndex;
    private bool isDragging;
    private Vector2 originPosition;

    public int SlotIndex => slotIndex;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        ResetRootRect();
    }

    public void Init(InventoryManager manager, Canvas rootCanvas, int index)
    {
        inventoryManager = manager;
        canvas = rootCanvas;
        slotIndex = index;

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : canvas.worldCamera;

        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(100f, 100f);
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;

        LayoutElement layout = GetComponent<LayoutElement>();
        if (layout == null)
            layout = gameObject.AddComponent<LayoutElement>();

        layout.ignoreLayout = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!iconImage.enabled)
            return;

        isDragging = true;
        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
        originPosition = rectTransform.anchoredPosition;

        MoveToMouse(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        MoveToMouse(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        isDragging = false;
        canvasGroup.blocksRaycasts = true;

        EquipmentSlotUI equipSlot = inventoryManager.GetEquipmentSlotUnderMouse(eventData);

        if (equipSlot != null)
        {
            bool equipped =
                inventoryManager.TryEquipItem(slotIndex, equipSlot);

            if (!equipped)
            {
                rectTransform.anchoredPosition =
                    inventoryManager.GetSlotPositionInItemParent(slotIndex);
            }

            return;
        }

        int targetIndex = inventoryManager.GetSlotIndexUnderMouse(eventData);

        if (targetIndex < 0)
        {
            rectTransform.anchoredPosition =
                inventoryManager.GetSlotPositionInItemParent(slotIndex);
            return;
        }

        inventoryManager.SwapSlot(slotIndex, targetIndex);
    }

    private void MoveToMouse(PointerEventData eventData)
    {
        RectTransform parentRect = inventoryManager.ItemIconParent;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            uiCamera,
            out Vector2 localPoint
        );

        rectTransform.anchoredPosition = localPoint;
    }

    public void SetIcon(InventorySlotData slotData)
    {
        if (slotData == null || slotData.IsEmpty)
        {
            iconImage.enabled = false;
            countText.text = "";
            return;
        }

        iconImage.enabled = true;
        iconImage.sprite = slotData.item.icon;
        iconImage.preserveAspect = true;

        countText.text = slotData.count > 1 ? slotData.count.ToString() : "";

        ResetChildRects();
    }

    public void SetSlotIndex(int index)
    {
        slotIndex = index;
    }

    public void SetAnchoredPosition(Vector2 pos)
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        rectTransform.anchoredPosition = pos;
    }

    private void ResetChildRects()
    {
        if (iconImage != null)
        {
            RectTransform iconRect = iconImage.GetComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.offsetMin = new Vector2(6, 6);
            iconRect.offsetMax = new Vector2(-6, -6);
            iconRect.localScale = Vector3.one;
            iconRect.localRotation = Quaternion.identity;
        }

        if (countText != null)
        {
            RectTransform textRect = countText.GetComponent<RectTransform>();
            textRect.localScale = Vector3.one;
            textRect.localRotation = Quaternion.identity;
        }
    }

    private void ResetRootRect()
    {
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
    }

}
