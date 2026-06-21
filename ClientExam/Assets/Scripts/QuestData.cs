using UnityEngine;
public enum QuestType
{
    OneTime,
    Daily
}

public enum QuestGoalType
{
    CollectItem,
    KillMonster,
    MineOre,
    TalkToNpc
}

[CreateAssetMenu(menuName = "Quest/Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("Basic")]
    public string questId;
    public string questName;
    public QuestType questType;

    [Header("Goal")]
    public QuestGoalType goalType;
    public ItemData targetItem;
    public int requiredAmount;

    [Header("Reward")]
    public ItemData rewardItem;
    public int rewardAmount;
    public int rewardGold;
}