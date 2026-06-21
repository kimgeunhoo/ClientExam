using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private InventoryManager inventoryManager;

    [Header("Quest Data")]
    [SerializeField] private List<QuestData> questDatas;
    private List<QuestProgress> quests = new List<QuestProgress>();

    private void Start()
    {
        InitQuests();
    }

    private void InitQuests()
    {
        quests.Clear();
    }

    private string GetToday()
    {
        return DateTime.Now.ToString("yyyy-MM-dd");
    }

    private void ResetDailyQuest(QuestProgress progress)
    {
        if (progress.quest.questType != QuestType.Daily)
            return;

        string today = GetToday();

        if(progress.lastRewardDate != today)
        {
            progress.currentAmount = 0;
            progress.isCompleted = false;
            progress.isRewarded = false;
        }
    }

    public void OnItemCollected(ItemData item, int amount)
    {
        foreach (QuestProgress progress in quests)
        {
            ResetDailyQuest(progress);

            if (progress.isCompleted)
                continue;

            QuestData quest = progress.quest;

            if (quest.goalType != QuestGoalType.CollectItem)
                continue;

            if (quest.targetItem != item)
                continue;

            progress.currentAmount += amount;

            if (progress.currentAmount >= quest.requiredAmount)
            {
                progress.currentAmount = quest.requiredAmount;
                progress.isCompleted = true;
            }
        }
    }

    public void ReceiveReward(QuestProgress progress)
    {
        ResetDailyQuest(progress);

        if (!progress.isCompleted || progress.isRewarded)
            return;

        QuestData quest = progress.quest;

        if (progress.quest.rewardItem != null && quest.rewardAmount > 0)
        {
            inventoryManager.AddItem(progress.quest.rewardItem, progress.quest.rewardAmount);
        }

        inventoryManager.AddGold(quest.rewardGold);

        if (quest.questType == QuestType.Daily)
        {
            progress.lastRewardDate = GetToday();
        }

        // Daily만 false 초기화하면 되는거같음
        //if (quest.questType == QuestType.OneTime)
        //{
        //    // 1회성은 isRewarded true 상태로 계속 유지
        //    // 저장 시스템에서 이 값을 저장해야 다시 못 받음
        //}
        progress.isRewarded = true;
    }
    public List<QuestProgress> GetQuests()
    {
        foreach (QuestProgress progress in quests)
        {
            ResetDailyQuest(progress);
        }

        return quests;
    }

    public List<QuestProgress> GetDailyQuests()
    {
        List<QuestProgress> result = new();

        foreach (QuestProgress progress in quests)
        {
            ResetDailyQuest(progress);

            if (progress.quest.questType == QuestType.Daily)
                result.Add(progress);
        }

        return result;
    }

    public List<QuestProgress> GetOneTimeQuests()
    {
        List<QuestProgress> result = new();

        foreach (QuestProgress progress in quests)
        {
            if (progress.quest.questType == QuestType.OneTime)
                result.Add(progress);
        }

        return result;
    }



}
