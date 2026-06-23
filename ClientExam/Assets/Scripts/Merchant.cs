using TMPro;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    [Header("Merchant")]
    [SerializeField] private string merchantName;

    [SerializeField] private ShopItemData[] sellItems;

    [Header("UI")]
    [SerializeField] private GameObject interactTextRoot;

    [SerializeField] private TextMeshProUGUI interactText;

    public string MerchantName => merchantName;
    public ShopItemData[] SellItems => sellItems;

    private void Awake()
    {
        HideInteractText();
    }

    private void Start()
    {
        interactText.text = $"E : {merchantName}와 거래하기";
    }

    public void ShowInteractText()
    {
        interactTextRoot.SetActive(true);
    }

    public void HideInteractText()
    {
        interactTextRoot.SetActive(false);
    }
}
