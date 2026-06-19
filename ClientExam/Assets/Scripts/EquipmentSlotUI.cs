using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private EquipSlotType slotType;
    [SerializeField] private Image iconImage;

    private InventoryManager inventoryManager;
    private EquipmentManager equipmentManager;
    private RectTransform iconRect;
    private CanvasGroup iconCanvasGroup;
    private Canvas rootCanvas;
    private Camera uiCamera;

    private ItemData equippedItem;
    private Vector2 originPosition;
    private bool isDragging;

    public EquipSlotType SlotType => slotType;
    public ItemData EquippedItem => equippedItem;
    private void Awake()
    {
        if (iconImage != null)
        {
            iconRect = iconImage.GetComponent<RectTransform>();
            iconCanvasGroup = iconImage.GetComponent<CanvasGroup>();

            if (iconCanvasGroup == null)
                iconCanvasGroup = iconImage.gameObject.AddComponent<CanvasGroup>();
        }

        SetIcon(null);
    }
    public void Init(InventoryManager inventory, EquipmentManager equipment, Canvas canvas)
    {
        inventoryManager = inventory;
        equipmentManager = equipment;
        rootCanvas = canvas;

        uiCamera = rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : rootCanvas.worldCamera;
    }
    public bool CanEquip(ItemData item)
    {
        if (item == null)
            return false;

        if (item.itemType != ItemType.Equipment)
            return false;

        return item.equipSlotType == slotType;
    }

    public void SetIcon(ItemData item)
    {
        equippedItem = item;

        if (iconImage == null)
            return;

        if (item == null)
        {
            iconImage.enabled = false;
            iconImage.sprite = null;
            return;
        }

        iconImage.enabled = true;
        iconImage.sprite = item.icon;
        iconImage.preserveAspect = true;

        iconRect.anchorMin = Vector2.zero;
        iconRect.anchorMax = Vector2.one;
        iconRect.offsetMin = new Vector2(6, 6);
        iconRect.offsetMax = new Vector2(-6, -6);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.localScale = Vector3.one;
        iconRect.localRotation = Quaternion.identity;
        iconRect.anchoredPosition = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (equippedItem == null)
            return;

        isDragging = true;
        originPosition = iconRect.anchoredPosition;

        iconCanvasGroup.blocksRaycasts = false;
        iconImage.transform.SetAsLastSibling();

        MoveIconToMouse(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        MoveIconToMouse(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        isDragging = false;
        iconCanvasGroup.blocksRaycasts = true;

        int inventoryIndex = inventoryManager.GetSlotIndexUnderMouse(eventData);

        if (inventoryIndex < 0)
        {
            iconRect.anchoredPosition = originPosition;
            return;
        }

        bool success = inventoryManager.TryUnequipItem(slotType, inventoryIndex);

        if (!success)
            iconRect.anchoredPosition = originPosition;
    }
    private void MoveIconToMouse(PointerEventData eventData)
    {
        RectTransform parentRect = iconRect.parent as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            uiCamera,
            out Vector2 localPoint);

        iconRect.anchoredPosition = localPoint;
    }
}
