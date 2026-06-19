using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private int capacity = 35;
    [SerializeField] private int gold = 10000;

    [Header("UI")]
    [SerializeField] private Canvas rootCanvas;
    [SerializeField] private RectTransform slotImageParent;
    [SerializeField] private RectTransform itemIconParent;
    [SerializeField] private ItemIconUI itemIconPrefab;

    [Header("Test Items")]
    [SerializeField] private ItemData testPickAxe;
    [SerializeField] private ItemData testOre;
    [SerializeField] private ItemData testSword;

    [Header("Equip Method")]
    [SerializeField] private EquipmentManager equipmentManager;
    [SerializeField] private RectTransform equipmentSlotParent;
    public int Gold => gold;

    private List<InventorySlotData> slots = new List<InventorySlotData>();
    private List<RectTransform> slotRects = new List<RectTransform>();
    private List<ItemIconUI> itemIcons = new List<ItemIconUI>();

    public RectTransform ItemIconParent => itemIconParent;

    private IEnumerator Start()
    {
        InitInventory();
        InitEquipmentSlots();

        CacheSlotRects();
        CreateItemIcons();

        AddItem(testPickAxe, 1);
        AddItem(testOre, 5);
        AddItem(testSword, 1);

        yield return null;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(slotImageParent);

        RefreshUI();
    }

    private void CreateItemIcons()
    {
        itemIcons.Clear();

        if (slotRects.Count < capacity)
        {
            Debug.LogError($"슬롯 UI 개수가 부족합니다. slotRects={slotRects.Count}, capacity={capacity}");
            return;
        }

        for (int i = 0; i < capacity; i++)
        {
            ItemIconUI icon = Instantiate(itemIconPrefab, itemIconParent);
            icon.Init(this, rootCanvas, i);

            icon.SetAnchoredPosition(GetSlotPositionInItemParent(i));
            
            itemIcons.Add(icon);
        }
    }

    private void CacheSlotRects()
    {
        slotRects.Clear();

        for (int i = 0; i < slotImageParent.childCount; i++)
        {
            RectTransform slotRect = slotImageParent.GetChild(i).GetComponent<RectTransform>();
            slotRects.Add(slotRect);

        }
    }

    private void InitInventory()
    {
        slots.Clear();
        for (int i = 0; i < capacity; i++)
        {
            slots.Add(new InventorySlotData());
        }
    }
    private void InitEquipmentSlots()
    {
        if (equipmentSlotParent == null)
            return;

        EquipmentSlotUI[] equipSlots =
            equipmentSlotParent.GetComponentsInChildren<EquipmentSlotUI>(true);

        foreach (EquipmentSlotUI slot in equipSlots)
        {
            slot.Init(this, equipmentManager, rootCanvas);
        }
    }
    public bool AddItem(ItemData item, int amount, bool refresh = true)
    {
        if (item == null || amount <= 0)
            return false;

        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].IsEmpty && slots[i].item == item)
            {
                int space = item.maxStack - slots[i].count;
                int addAmount = Mathf.Min(space, amount);

                if (addAmount <= 0)
                    continue;

                slots[i].count += addAmount;
                amount -= addAmount;

                if (amount <= 0)
                {
                    if (refresh) RefreshUI();
                    return true;
                }
            }
        }

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsEmpty)
            {
                int addAmount = Mathf.Min(item.maxStack, amount);

                slots[i].SetItem(item, addAmount);
                amount -= addAmount;

                if (amount <= 0)
                {
                    if (refresh) RefreshUI();
                    return true;
                }
            }
        }

        if (refresh) 
            RefreshUI();
        return false;
    }
    public int GetSlotIndexUnderMouse(PointerEventData eventData)
    {
        Camera cam = rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay
        ? null
        : rootCanvas.worldCamera;

        for (int i = 0; i < slotRects.Count; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(
                    slotRects[i],
                    eventData.position,
                    cam))
            {
                return i;
            }
        }

        return -1;
    }

    public bool RemoveItem(ItemData item, int amount, bool refresh = true)
    {
        if (item == null || amount <= 0)
            return false;

        int remainAmount = amount;

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsEmpty)
                continue;

            if (slots[i].item != item)
                continue;

            int removeAmount = Mathf.Min(slots[i].count, remainAmount);

            slots[i].count -= removeAmount;
            remainAmount -= removeAmount;

            if (slots[i].count <= 0)
                slots[i].Clear();

            if (remainAmount <= 0)
            {
                if (refresh)
                    RefreshUI();

                return true;
            }
        }

        if (refresh)
            RefreshUI();

        return false;
    }

    public Vector2 GetSlotPositionInItemParent(int index)
    {
        Camera cam = rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : rootCanvas.worldCamera;

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, slotRects[index].position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            itemIconParent,
            screenPoint,
            cam,
            out Vector2 localPoint
        );

        return localPoint;
    }
    public void RefreshAfterOpen()
    {
        StartCoroutine(RefreshAfterOpenRoutine());
    }

    private IEnumerator RefreshAfterOpenRoutine()
    {
        yield return null;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(slotImageParent);
        LayoutRebuilder.ForceRebuildLayoutImmediate(itemIconParent);

        RefreshUI();
    }

    public void SwapSlot(int fromIndex, int toIndex)
    {
        if (fromIndex == toIndex)
        {
            RefreshUI();
            return;
        }

        InventorySlotData temp = slots[fromIndex];
        slots[fromIndex] = slots[toIndex];
        slots[toIndex] = temp;

        RefreshUI();
    }

    public int GetItemCount(ItemData item)
    {
        int total = 0;

        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].IsEmpty && slots[i].item == item)
                total += slots[i].count;
        }

        return total;
    }
    private void RefreshUI()
    {
        for (int i = 0; i < itemIcons.Count; i++)
        {
            itemIcons[i].SetSlotIndex(i);
            itemIcons[i].SetAnchoredPosition(GetSlotPositionInItemParent(i));
            itemIcons[i].SetIcon(slots[i]);
        }
    }

    public bool SpendGold(int amount)
    {
        if (gold < amount)
            return false;

        gold -= amount;
        return true;
    }

    public void AddGold(int amount)
    {
        gold += amount;
    }
    public List<InventorySlotData> GetSlots()
    {
        return slots;
    }

    public EquipmentSlotUI GetEquipmentSlotUnderMouse(PointerEventData eventData)
    {
        Camera cam = rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : rootCanvas.worldCamera;

        for (int i = 0; i < equipmentSlotParent.childCount; i++)
        {
            EquipmentSlotUI equipSlot =
                equipmentSlotParent.GetChild(i).GetComponent<EquipmentSlotUI>();

            if (equipSlot == null)
                continue;

            RectTransform rect = equipSlot.GetComponent<RectTransform>();

            if (RectTransformUtility.RectangleContainsScreenPoint(
                    rect,
                    eventData.position,
                    cam))
            {
                return equipSlot;
            }
        }

        return null;
    }

    public bool TryEquipItem(int inventoryIndex, EquipmentSlotUI equipSlot)
    {
        if (equipSlot == null)
            return false;

        InventorySlotData slotData = slots[inventoryIndex];

        if (slotData == null || slotData.IsEmpty)
            return false;

        ItemData newItem = slotData.item;

        if (!equipSlot.CanEquip(newItem))
        {
            Debug.Log("이 슬롯에 장착할 수 없는 아이템입니다.");
            return false;
        }

        bool success = equipmentManager.Equip(newItem, out ItemData oldItem);

        if (!success)
            return false;

        slotData.Clear();

        if (oldItem != null)
        {
            slotData.SetItem(oldItem, 1);
        }

        equipSlot.SetIcon(newItem);

        RefreshUI();

        return true;
    }
    public bool TryUnequipItem(EquipSlotType slotType, int inventoryIndex)
    {
        if (equipmentManager == null)
            return false;

        if (inventoryIndex < 0 || inventoryIndex >= slots.Count)
            return false;

        if (!slots[inventoryIndex].IsEmpty)
        {
            Debug.Log("빈 인벤토리 슬롯에만 해제할 수 있습니다.");
            return false;
        }

        ItemData equippedItem = equipmentManager.GetEquippedItem(slotType);

        if (equippedItem == null)
            return false;

        slots[inventoryIndex].SetItem(equippedItem, 1);

        equipmentManager.Unequip(slotType);

        EquipmentSlotUI equipSlot = GetEquipmentSlotByType(slotType);
        if (equipSlot != null)
            equipSlot.SetIcon(null);

        RefreshUI();

        return true;
    }
    private EquipmentSlotUI GetEquipmentSlotByType(EquipSlotType slotType)
    {
        if (equipmentSlotParent == null)
            return null;

        EquipmentSlotUI[] equipSlots =
            equipmentSlotParent.GetComponentsInChildren<EquipmentSlotUI>(true);

        foreach (EquipmentSlotUI slot in equipSlots)
        {
            if (slot.SlotType == slotType)
                return slot;
        }

        return null;
    }
}
