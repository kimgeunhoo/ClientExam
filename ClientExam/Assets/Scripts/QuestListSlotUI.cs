using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestListSlotUI : MonoBehaviour
{
    [SerializeField] private Button slotButton;
    [SerializeField] private Button trackButton;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI requireText;

    [SerializeField] private GameObject checkedImage;

    private QuestProgress progress;
    private Action<QuestProgress> onClickSlot;
    private Action<QuestProgress> onClickTrack;

    public void Init(QuestProgress _progress, Action<QuestProgress> _onClickSlot, Action<QuestProgress> _onClickTrack)
    {
        this.progress = _progress;
        this.onClickSlot = _onClickSlot;
        this.onClickTrack = _onClickTrack;

        QuestData quest = _progress.quest;

        titleText.text = quest.questName;
        requireText.text =
            $"{quest.targetItem.itemName} ({_progress.currentAmount} / {quest.requiredAmount})";

        checkedImage.SetActive(_progress.isTracked);

        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(() => this.onClickSlot?.Invoke(this.progress));

        trackButton.onClick.RemoveAllListeners();
        trackButton.onClick.AddListener(() => this.onClickTrack?.Invoke(this.progress));
    }
}
