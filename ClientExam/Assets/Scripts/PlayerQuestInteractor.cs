using TMPro;
using UnityEngine;

public class PlayerQuestInteractor : MonoBehaviour
{

    private QuestNpc nearestNpc;

    public bool TryInteract()
    {
        if (nearestNpc == null)
        {
            Debug.Log("대화 가능한 NPC 없음");
            return false;
        }

        Debug.Log($"대화 실행: {nearestNpc.name}");
        nearestNpc.OpenDialogue();
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        QuestNpc npc = other.GetComponentInParent<QuestNpc>();

        if (npc == null)
            return;

        nearestNpc = npc;
        nearestNpc.ShowPrompt();

        Debug.Log($"NPC 범위 진입: {npc.name}");
    }

    private void OnTriggerExit(Collider other)
    {
        QuestNpc npc = other.GetComponentInParent<QuestNpc>();

        if (npc == null)
            return;

        if (nearestNpc == npc)
        {
            nearestNpc.HidePrompt();
            nearestNpc = null;

            Debug.Log($"NPC 범위 이탈: {npc.name}");
        }
    }
}
