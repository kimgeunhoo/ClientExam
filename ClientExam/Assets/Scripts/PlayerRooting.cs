
using UnityEngine;

public class PlayerRooting : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private QuestManager questManager;

    [Header("Pickup")]
    [SerializeField] private float pickupRange = 2f;
    [SerializeField] private LayerMask itemLayer;

    private DropItem nearestItem;
    private DropItem previousNearestItem;
    private void Update()
    {
        FindNearestItem();
        UpdatePickupText();
    }

    public bool TryInteract()
    {
        return TryPickUp();
    }

    private void FindNearestItem()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange, itemLayer);

        nearestItem = null;
        float nearestDistance = float.MaxValue;
        foreach (Collider hit in hits)
        {
            DropItem item = hit.GetComponentInParent<DropItem>();
            if (item == null)
                continue;

            float distance = Vector3.Distance(transform.position, item.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestItem = item;
            }
        }
    }
    private void UpdatePickupText()
    {
        if (previousNearestItem != null && previousNearestItem != nearestItem)
            previousNearestItem.HidePickupText();

        if (nearestItem != null)
            nearestItem.ShowPickupText();

        previousNearestItem = nearestItem;
    }

    private bool TryPickUp()
    {
        if (nearestItem == null)
        {
            Debug.Log("줍기 실패: nearestItem 없음");
            return false;
        }

        Debug.Log($"줍기 입력 감지: {nearestItem.name}");

        ItemData itemData = nearestItem.ItemData;
        int count = nearestItem.Amount;

        bool success = nearestItem.PickUp(inventoryManager);
        if (questManager != null)
        {
            questManager.OnItemCollected(itemData, count);
        }

        if (!success)
        {
            Debug.Log("아이템을 주울 수 없습니다. 인벤토리가 가득 찼거나 ItemData가 없습니다.");
            return false;
        }
        return success;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
