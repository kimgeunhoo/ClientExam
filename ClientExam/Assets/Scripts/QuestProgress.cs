using UnityEngine;

[System.Serializable]
public class QuestProgress
{
    public QuestData quest;

    public int currentAmount;
    public bool isCompleted;
    public bool isRewarded;

    public bool isAccepted;
    public bool isTracked;

    public string lastRewardDate;

    public bool CanComplete => currentAmount >= quest.requiredAmount;
}
