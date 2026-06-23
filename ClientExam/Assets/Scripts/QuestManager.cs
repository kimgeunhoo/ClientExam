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

    public event Action OnQuestChanged;

    [Header("DayQuest Test")]
    [SerializeField] private int testDayOffset = 0;

    [ContextMenu("Force Daily Reset Check")]
    public void ForceDailyResetCheck()
    {
        foreach (QuestProgress progress in quests)
        {
            ResetDailyQuest(progress);
        }

        RefreshQuestProgressFromInventory(false);

        OnQuestChanged?.Invoke();
        Debug.Log($"일일 퀘스트 강제 체크 실행 / Today: {GetToday()}");
    }
    public void AddTestDay()
    {
        testDayOffset++;
        ForceDailyResetCheck();
    }

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
        return DateTime.Now.AddDays(testDayOffset).ToString("yyyy-MM-dd");
    }

    private void ResetDailyQuest(QuestProgress progress)
    {
        if (progress.quest.questType != QuestType.Daily)
            return;

        string today = GetToday();

        if (progress.isRewarded && progress.lastRewardDate != today)
        {
            progress.isAccepted = false;
            progress.isCompleted = false;
            progress.isRewarded = false;
            progress.isTracked = false;
            progress.currentAmount = 0;
        }
        Debug.Log($"Daily 리셋 완료: {progress.quest.questName}");
    }

    public void RefreshQuestProgressFromInventory(bool notify = true)
    {
        foreach (QuestProgress progress in quests)
        {
            if (!progress.isAccepted)
                continue;

            QuestData quest = progress.quest;

            if (quest.goalType != QuestGoalType.CollectItem &&
               quest.goalType != QuestGoalType.MineOre)
                continue;

            if (quest.targetItem == null)
                continue;

            int count = inventoryManager.GetItemCount(quest.targetItem);

            progress.currentAmount = Mathf.Min(count, quest.requiredAmount);
            progress.isCompleted = progress.currentAmount >= quest.requiredAmount;

            Debug.Log($"퀘스트 보유량 동기화: {quest.questName} {progress.currentAmount}/{quest.requiredAmount}");

        }

        if (notify)
            OnQuestChanged?.Invoke();
    }

    public void OnItemCollected(ItemData item, int amount)
    {
        RefreshQuestProgressFromInventory();
    }

    public void ReceiveReward(QuestProgress progress)
    {
        if (progress == null)
            return;

        ResetDailyQuest(progress);
        RefreshQuestProgressFromInventory(false);

        if (!progress.isCompleted || progress.isRewarded)
            return;

        QuestData quest = progress.quest;

        if (quest.targetItem != null && quest.requiredAmount > 0)
        {
            bool removed = inventoryManager.RemoveItem(
                quest.targetItem,
                quest.requiredAmount
            );

            if (!removed)
            {
                Debug.Log("퀘스트 요구 아이템 제거 실패");
                return;
            }
        }

        if (quest.rewardItem != null && quest.rewardAmount > 0)
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
        progress.isAccepted = false;

        if (quest.questType == QuestType.Daily)
            progress.lastRewardDate = GetToday();

        OnQuestChanged?.Invoke();
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
    public bool IsQuestRewarded(QuestData quest)
    {
        QuestProgress progress = GetQuestProgress(quest);

        if (progress == null)
            return false;

        ResetDailyQuest(progress);

        return progress.isRewarded;
    }

    public bool IsDailyQuestCompletedToday(QuestData quest)
    {
        if (quest.questType != QuestType.Daily)
            return false;

        QuestProgress progress = GetQuestProgress(quest);

        if (progress == null)
            return false;

        return progress.isRewarded &&
               progress.lastRewardDate == GetToday();
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

        ResetDailyQuest(progress);
        SyncQuestProgressWithInventory(progress);

        progress.isAccepted = true;
        progress.isTracked = true;


        RefreshQuestProgressFromInventory(false);

        OnQuestChanged?.Invoke();

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
