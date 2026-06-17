using NUnit.Framework.Internal.Execution;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRooting : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private InventoryManager inventoryManager;

    [Header("Pickup")]
    [SerializeField] private float pickupRange = 2f;
    [SerializeField] private LayerMask itemLayer;

    private DropItem nearestItem;

    private void Update()
    {
        FindNearestItem();
    }
    public void OnPickup(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        TryPickUp();
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

    private void TryPickUp()
    {
        if (nearestItem == null)
            return;

        bool success = nearestItem.PickUp(inventoryManager);

        if (!success)
        {
            Debug.Log("아이템을 주울 수 없습니다. 인벤토리가 가득 찼거나 ItemData가 없습니다.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
