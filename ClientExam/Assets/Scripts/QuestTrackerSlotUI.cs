using TMPro;
using UnityEngine;

public class QuestTrackerSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI requireText;

    public void Init(QuestProgress progress)
    {
        QuestData quest = progress.quest;

        titleText.text = quest.questName;
        requireText.text = $"{quest.targetItem.itemName} ({progress.currentAmount} / {quest.requiredAmount})";

    }
}
