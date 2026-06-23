using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuestNpc : MonoBehaviour
{
    [SerializeField] private string npcName = "Admin";
    [SerializeField] private QuestDialogueUI dialogueUI;
    [SerializeField] private NpcPromptUI promptUI;
    [SerializeField] private QuestManager questManager;
    [Header("0 = OneTime, 1 = Daily")]
    [SerializeField] private QuestData[] questData;
    private void Start()
    {
        HidePrompt();
    }
    public void ShowPrompt()
    {
        if (promptUI != null)
            promptUI.Show($"E : {npcName}과 대화하기");
    }

    public void HidePrompt()
    {
        if (promptUI != null)
            promptUI.Hide();
    }

    public void OpenDialogue()
    {
        Debug.Log($"NPC 대화 시도: {npcName}");

        if (dialogueUI == null)
        {
            Debug.LogError($"{name} : Dialogue UI가 연결되지 않았습니다.");
            return;
        }

        if (questData == null)
        {
            Debug.LogError($"{name} : QuestData가 연결되지 않았습니다.");
            return;
        }

        QuestData selectedQuest = GetAvailableQuest();
        HidePrompt();

        if (selectedQuest == null)
        {
            dialogueUI.OpenNoQuest(
                npcName,
                "오늘은 더 부탁할 일이 없네."
            );

            return;
        }
        dialogueUI.Open(npcName, selectedQuest);
    }

    private QuestData GetAvailableQuest()
    {
        QuestData oneTimeQuest = GetQuest(0);
        QuestData dailyQuest = GetQuest(1);

        if (oneTimeQuest != null &&
            !questManager.IsQuestRewarded(oneTimeQuest))
        {
            return oneTimeQuest;
        }

        if (dailyQuest != null)
        {
            if (!questManager.IsDailyQuestCompletedToday(dailyQuest))
                return dailyQuest;
        }

        return null;
    }

    private QuestData GetQuest(int index)
    {
        if (questData == null)
            return null;

        if (index < 0 || index >= questData.Length)
            return null;

        return questData[index];
    }
}
