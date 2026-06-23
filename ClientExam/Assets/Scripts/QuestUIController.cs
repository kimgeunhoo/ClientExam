using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QuestUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private QuestManager questManager;

    [Header("Input")]
    [SerializeField] private PlayerInputAction input;

    [Header("Panels")]
    [SerializeField] private GameObject questDetailPanel;


    [Header("Quest List")]
    [SerializeField] private Transform questListParent;
    [SerializeField] private QuestListSlotUI questListSlotPF;

    [Header("Tracker")]
    [SerializeField] private Transform trackerParent;
    [SerializeField] private QuestTrackerSlotUI trackerSlotPF;

    [Header("Detail")]
    [SerializeField] private TextMeshProUGUI detailTitleText;
    [SerializeField] private TextMeshProUGUI detailDescriptionText;
    [SerializeField] private TextMeshProUGUI requireText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Button closeButton;

    [SerializeField] private Image rewardImage1;
    [SerializeField] private Image rewardImage2;

    private QuestProgress selectedQuest;

    private void Awake()
    {
        questDetailPanel.SetActive(false);
        closeButton.onClick.AddListener(CloseQuestPanel);
    }
    private void OnEnable()
    {
        if (questManager != null)
            questManager.OnQuestChanged += RefreshAll;
    }

    private void OnDisable()
    {
        if (questManager != null)
            questManager.OnQuestChanged -= RefreshAll;
    }

    public void ToggleQuestPanel()
    {
        Debug.Log("Äù½ºÆ® Toggle ÀÔ·ÂµÊ");

        if (questDetailPanel.activeSelf)
            CloseQuestPanel();
        else
            OpenQuestPanel();
    }

    private void OpenQuestPanel()
    {
        Debug.Log("Äù½ºÆ® Ã¢ ¿­¸²");


        questDetailPanel.SetActive(true);
        RefreshQuestList();
        RefreshTracker();
    }

    private void CloseQuestPanel()
    {
        questDetailPanel.SetActive(false);
    }
    private void RefreshQuestList()
    {
        ClearChildren(questListParent);

        List<QuestProgress> quests = questManager.GetQuests();
        Debug.Log($"Äù½ºÆ® Ã¢ °»½Å: {quests.Count}");

        foreach (QuestProgress progress in quests)
        {
            Debug.Log($"Ç¥½Ã Äù½ºÆ®: {progress.quest.questName}");

            QuestListSlotUI slot = Instantiate(questListSlotPF, questListParent);

            slot.Init(
                progress,
                OnClickQuestSlot,
                OnClickTrackToggle
            );
        }
        if (questManager.GetQuests().Count == 0)
        {
            selectedQuest = null;
            ClearQuestDetail();
        }
    }

    private void ClearQuestDetail()
    {
        detailTitleText.text = "";
        detailDescriptionText.text = "";
        requireText.text = "";
        rewardText.text = "";

        rewardImage1.sprite = null;
        rewardImage2.sprite = null;

        rewardImage1.gameObject.SetActive(false);
        rewardImage2.gameObject.SetActive(false);
    }

    private void RefreshAll()
    {
        questManager.RefreshQuestProgressFromInventory(false);

        RefreshQuestList();
        RefreshTracker();

        if (selectedQuest == null)
        {
            ClearQuestDetail();
            return;
        }

        if (selectedQuest.isRewarded)
        {
            selectedQuest = null;
            ClearQuestDetail();
            return;
        }

        ShowQuestDetail(selectedQuest);
    }

    private void RefreshTracker()
    {
        ClearChildren(trackerParent);

        List<QuestProgress> trackedQuests = questManager.GetTrackedQuests();

        foreach (QuestProgress progress in trackedQuests)
        {
            QuestTrackerSlotUI slot = Instantiate(trackerSlotPF, trackerParent);
            slot.Init(progress);
        }
    }

    private void OnClickQuestSlot(QuestProgress progress)
    {
        ShowQuestDetail(progress);
    }

    private void OnClickTrackToggle(QuestProgress progress)
    {
        questManager.ToggleTracked(progress);

        RefreshQuestList();
        RefreshTracker();
    }

    private void ShowQuestDetail(QuestProgress progress)
    {
        selectedQuest = progress;

        QuestData quest = progress.quest;

        detailTitleText.text = quest.questName;
        detailDescriptionText.text = quest.description;
        requireText.text =
           $"{quest.targetItem.itemName} : ({progress.currentAmount} / {quest.requiredAmount})";

        rewardText.text =
            $"{quest.rewardGold} Gold, {quest.rewardItem.itemName} x {quest.rewardAmount}";
    }

    private void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent)
            Destroy(child.gameObject);
    }
}
