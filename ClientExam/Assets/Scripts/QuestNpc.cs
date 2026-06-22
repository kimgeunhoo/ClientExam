using UnityEngine;
using UnityEngine.InputSystem;

public class QuestNpc : MonoBehaviour
{
    [SerializeField] private string npcName = "Admin";
    [SerializeField] private QuestData questData;
    [SerializeField] private QuestDialogueUI dialogueUI;
    [SerializeField] private NpcPromptUI promptUI;
    private void Start()
    {
        HidePrompt();
    }
    public void ShowPrompt()
    {
        if (promptUI != null)
            promptUI.Show($"E : Talk to {npcName}");
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

        HidePrompt();
        dialogueUI.Open(npcName, questData);
    }
}
