using UnityEngine;

[System.Serializable]
public class QuestProgress
{
    public QuestData quest;

    public int currentAmount;
    public bool isCompleted;
    public bool isRewarded;

    public string lastRewardDate;

    public bool CanComplete => currentAmount >= quest.requiredAmount;
}
