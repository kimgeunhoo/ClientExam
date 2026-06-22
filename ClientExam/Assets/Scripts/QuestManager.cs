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
            if (!progress.isAccepted)
                continue;

            ResetDailyQuest(progress);

            if (progress.isCompleted)
                continue;

            QuestData quest = progress.quest;

            Debug.Log($"비교: QuestTarget={quest.targetItem?.itemName}, Item={item.itemName}");

            if (quest.goalType != QuestGoalType.CollectItem)
                continue;

            if (quest.targetItem != item)
                continue;

            progress.currentAmount += amount;

            if (progress.currentAmount >= quest.requiredAmount)
            {
                progress.currentAmount = quest.requiredAmount;
                progress.isCompleted = true;
                Debug.Log($"퀘스트 완료: {quest.questName}");
            }
            else
            {
                Debug.Log($"퀘스트 진행도: {progress.currentAmount}/{quest.requiredAmount}");
            }
        }
    }

    public void ReceiveReward(QuestProgress progress)
    {
        if (progress == null)
            return;

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
        progress.isTracked = false;

        if (quest.questType == QuestType.Daily)
            progress.lastRewardDate = GetToday();

    }
    public List<QuestProgress> GetQuests()
    {
        List<QuestProgress> result = new();

        foreach (QuestProgress progress in quests)
        {
            ResetDailyQuest(progress);

            if (!progress.isAccepted)
                continue;

            if (progress.isRewarded)
                continue;

            result.Add(progress);
        }

        return result;
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

    public void ToggleTracked(QuestProgress progress)
    {
        progress.isTracked = !progress.isTracked;
    }

    public List<QuestProgress> GetTrackedQuests()
    {
        List<QuestProgress> result = new();

        foreach (QuestProgress progress in quests)
        {
            ResetDailyQuest(progress);

            if (!progress.isAccepted)
                continue;

            if (!progress.isTracked)
                continue;

            if (progress.isRewarded)
                continue;

            result.Add(progress);
        }

        return result;
    }

    public bool HasQuest(QuestData quest)
    {
        return quests.Exists(q => q.quest == quest);
    }

    public QuestProgress GetQuestProgress(QuestData quest)
    {
        return quests.Find(q => q.quest == quest);
    }
    public void AcceptQuest(QuestData quest)
    {
        if (quest == null)
        {
            Debug.LogError("AcceptQuest 실패: quest가 null입니다.");
            return;
        }

        QuestProgress progress = GetQuestProgress(quest);

        if (progress == null)
        {
            progress = new QuestProgress
            {
                quest = quest,
                currentAmount = 0,
                isCompleted = false,
                isRewarded = false,
                isTracked = true,
                lastRewardDate = ""
            };

            quests.Add(progress);
        }
        else
        {
            progress.isAccepted = true;
            progress.isTracked = true;
        }

        if (progress.isAccepted)
            return;

        progress.isAccepted = true;
        progress.isTracked = true;

        SyncQuestProgressWithInventory(progress);

        Debug.Log($"퀘스트 수락됨: {quest.questName}");
    }

    private void SyncQuestProgressWithInventory(QuestProgress progress)
    {
        QuestData quest = progress.quest;

        if (quest.goalType != QuestGoalType.CollectItem &&
            quest.goalType != QuestGoalType.MineOre)
            return;

        if (quest.targetItem == null)
            return;

        int count = inventoryManager.GetItemCount(quest.targetItem);

        progress.currentAmount = Mathf.Min(count, quest.requiredAmount);

        if (progress.currentAmount >= quest.requiredAmount)
            progress.isCompleted = true;

        Debug.Log($"인벤토리 동기화: {quest.questName} {progress.currentAmount}/{quest.requiredAmount}");
    }

    public bool CanReceiveReward(QuestData quest)
    {
        QuestProgress progress = GetQuestProgress(quest);

        if (progress == null)
            return false;

        return progress.isCompleted && !progress.isRewarded;
    }
}
