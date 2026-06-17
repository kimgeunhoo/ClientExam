using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private int capacity = 35;

    [Header("UI")]
    [SerializeField] private Canvas rootCanvas;
    [SerializeField] private RectTransform slotImageParent;
    [SerializeField] private RectTransform itemIconParent;
    [SerializeField] private ItemIconUI itemIconPrefab;

    [Header("Test Items")]
    [SerializeField] private ItemData testGold;
    [SerializeField] private ItemData testPotion;
    [SerializeField] private ItemData testOre;
    [SerializeField] private ItemData testSword;

    private List<InventorySlotData> slots = new List<InventorySlotData>();
    private List<RectTransform> slotRects = new List<RectTransform>();
    private List<ItemIconUI> itemIcons = new List<ItemIconUI>();

    public RectTransform ItemIconParent => itemIconParent;

    private IEnumerator Start()
    {
        InitInventory();

        CacheSlotRects();
        CreateItemIcons();

        AddItem(testGold, 100);
        AddItem(testPotion, 5);
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

    private void RefreshUI()
    {
        for (int i = 0; i < itemIcons.Count; i++)
        {
            itemIcons[i].SetSlotIndex(i);
            itemIcons[i].SetAnchoredPosition(GetSlotPositionInItemParent(i));
            itemIcons[i].SetIcon(slots[i]);
        }
    }

    
}
