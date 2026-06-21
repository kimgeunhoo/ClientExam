using TMPro;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    [Header("Item Value")]
    [SerializeField] private ItemData itemData;
    [SerializeField] private int amount = 1;

    [Header("Pickup UI")]
    [SerializeField] private GameObject pickupTextRoot;
    [SerializeField] private TextMeshProUGUI pickupText;

    [Header("Quest Manager")]
    [SerializeField] private QuestManager questManager;

    public ItemData ItemData => itemData;
    public int Amount => amount;


    private void Awake()
    {
        HidePickupText();
    }

    private void Start()
    {
        RefreshPickupText();
    }

    public void ShowPickupText()
    {
        RefreshPickupText();

        if (pickupTextRoot != null)
            pickupTextRoot.SetActive(true);
    }

    public void HidePickupText()
    {
        if (pickupTextRoot != null)
            pickupTextRoot.SetActive(false);
    }
    private void RefreshPickupText()
    {
        if (pickupText == null || itemData == null)
            return;

        pickupText.text = $"F : Get {itemData.itemName} ";
    }
    public void SetAmount(int newAmount)
    {
        amount = Mathf.Max(1, newAmount);
    }

    public bool PickUp(InventoryManager inventory)
    {
        if (inventory == null)
        {
            //Debug.LogError("PickUp 실패: InventoryManager가 연결되지 않음");
            return false;
        }

        if (itemData == null)
        {
           // Debug.LogError("PickUp 실패: DropItem의 ItemData가 비어있음");
            return false;
        }

        //Debug.Log($"PickUp 시도: {itemData.name}, amount={amount}");

        bool success = inventory.AddItem(itemData, amount);
        questManager.OnItemCollected(itemData, amount);

       // Debug.Log($"AddItem 결과: {success}");

        if (success)
            Destroy(gameObject);

        return success;
    }
}
