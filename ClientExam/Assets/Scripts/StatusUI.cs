using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private CharacterStats characterStats;

    [Header("HP")]
    [SerializeField] private Image hpFill;
    //[SerializeField] private TextMeshProUGUI hpText;

    [Header("Stamina")]
    [SerializeField] private Image staminaFill;
    //[SerializeField] private TextMeshProUGUI staminaText;

    [Header("Gold")]
    [SerializeField] private TextMeshProUGUI goldText;

    private float currentHp;
    private float currentStamina;

    private void Start()
    {
        currentHp = characterStats.MaxHp;
        currentStamina = characterStats.MaxStamina;

        RefreshAll();
    }

    private void Update()
    {
        RefreshAll();
    }

    public void RefreshAll()
    {
        RefreshHp();
        RefreshStamina();
        RefreshGold();
    }

    private void RefreshHp()
    {
        float maxHp = characterStats.MaxHp;

        if (hpFill != null)
            hpFill.fillAmount = currentHp / maxHp;

        //if (hpText != null)
        //    hpText.text = $"{Mathf.RoundToInt(currentHp)} / {Mathf.RoundToInt(maxHp)}";
    }

    private void RefreshStamina()
    {
        float maxStamina = characterStats.MaxStamina;

        if (staminaFill != null)
            staminaFill.fillAmount = currentStamina / maxStamina;

        //if (staminaText != null)
        //    staminaText.text = $"{Mathf.RoundToInt(currentStamina)} / {Mathf.RoundToInt(maxStamina)}";
    }

    private void RefreshGold()
    {
        if (goldText != null)
            goldText.text = inventoryManager.Gold.ToString();
    }
}
