using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestDialogueUI : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;

    [Header("Base UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Request Panel")]
    [SerializeField] private GameObject requestPanel;
    [SerializeField] private TextMeshProUGUI requestTitleText;
    [SerializeField] private TextMeshProUGUI requestDescriptionText;
    [SerializeField] private TextMeshProUGUI requestRequireText;
    [SerializeField] private TextMeshProUGUI requestRewardText;
    [SerializeField] private Image rewardImage1;
    [SerializeField] private Image rewardImage2;

    [Header("Reward Panel")]
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private TextMeshProUGUI rewardTitleText;
    [SerializeField] private TextMeshProUGUI rewardDescriptionText;
    [SerializeField] private TextMeshProUGUI rewardRequireText;
    [SerializeField] private TextMeshProUGUI rewardRewardText;

    [Header("Buttons")]
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button getRewardButton;
    [SerializeField] private Button disAgreeButton;
    [SerializeField] private Button closeButton;


    private QuestData currentQuest;

    private void Awake()
    {
        requestPanel.SetActive(false);
        rewardPanel.SetActive(false);
        panel.SetActive(false);
        acceptButton.onClick.AddListener(OnClickAccept);
        getRewardButton.onClick.AddListener(OnClickReward);
        disAgreeButton.onClick.AddListener(Close);
        closeButton.onClick.AddListener(Close);
    }

    public void Open(string npcName, QuestData quest)
    {
        currentQuest = quest;
        Debug.Log($"대화 UI 열림: {npcName}, Quest: {quest?.questName}");
        panel.SetActive(true);
        npcNameText.text = npcName;

        Refresh();
    }

    private void Refresh()
    {
        if (currentQuest == null)
        {
            Debug.LogError("currentQuest가 null입니다.");
            return;
        }

        QuestProgress progress = questManager.GetQuestProgress(currentQuest);

       
        if (progress != null && progress.isRewarded)
        {
            dialogueText.text = "이미 도움을 받았네. 고맙군.";

            requestPanel.SetActive(false);
            rewardPanel.SetActive(false);

            acceptButton.gameObject.SetActive(false);
            getRewardButton.gameObject.SetActive(false);

            return;
        }

        bool hasQuest = progress != null && progress.isAccepted;
        bool canReward = hasQuest && progress.isCompleted && !progress.isRewarded;

        if (!hasQuest)
        {
            requestPanel.SetActive(true);
            rewardPanel.SetActive(false);

            dialogueText.text = "도움이 필요하네. 이 일을 맡아주겠나?";
            acceptButton.gameObject.SetActive(true);
            getRewardButton.gameObject.SetActive(false);
            SetRequestPanel(null);
            return;
        }

        if (!progress.isCompleted)
        {
            rewardPanel.SetActive(false);

            dialogueText.text = "아직 일이 끝나지 않은 것 같군.";
            acceptButton.gameObject.SetActive(false);
            getRewardButton.gameObject.SetActive(false);
            SetRequestPanel(progress);
            return;
        }

        if (canReward)
        {
            requestPanel.SetActive(false);
            rewardPanel.SetActive(true);

            dialogueText.text = "잘 해냈군. 약속한 보상을 주겠네.";
            acceptButton.gameObject.SetActive(false);
            getRewardButton.gameObject.SetActive(true);
            SetRewardPanel(progress);
            return;
        }

    }

    private void SetRequestPanel(QuestProgress progress = null)
    {
        int current = progress != null ? progress.currentAmount : 0;

        requestTitleText.text = currentQuest.questName;
        requestDescriptionText.text = currentQuest.description;

        requestRequireText.text =
            $"{currentQuest.targetItem.itemName} ({current} / {currentQuest.requiredAmount})";

        requestRewardText.text =
            $"{currentQuest.rewardGold} Gold, {currentQuest.rewardItem.itemName} x {currentQuest.rewardAmount}";

        SetRewardImages(rewardImage1, rewardImage2);
    }

    private void SetRewardPanel(QuestProgress progress)
    {
        rewardTitleText.text = currentQuest.questName;
        rewardDescriptionText.text = "퀘스트 완료 보상";

        rewardRequireText.text =
            $"{currentQuest.targetItem.itemName} ({progress.currentAmount} / {currentQuest.requiredAmount})";

        rewardRewardText.text =
            $"{currentQuest.rewardGold} Gold, {currentQuest.rewardItem.itemName} x {currentQuest.rewardAmount}";

        SetRewardImages(rewardImage1, rewardImage2);
    }

    private void SetRewardImages(Image image1, Image image2)
    {
        image1.gameObject.SetActive(false);
        image2.gameObject.SetActive(false);

        if (currentQuest.rewardItem != null)
        {
            image1.sprite = currentQuest.rewardItem.icon;
            image1.gameObject.SetActive(true);
        }

    }
    private void OnClickAccept()
    {
        Debug.Log($"Accept 클릭: {currentQuest?.questName}");
        questManager.AcceptQuest(currentQuest);
        Close();
    }

    private void OnClickReward()
    {
        QuestProgress progress = questManager.GetQuestProgress(currentQuest);

        if (progress == null)
            return;

        questManager.ReceiveReward(progress);
        Refresh();
        Close();
    }

    private void Close()
    {
        panel.SetActive(false);

        if (requestPanel != null)
            requestPanel.SetActive(false);

        if (rewardPanel != null)
            rewardPanel.SetActive(false);

        acceptButton.gameObject.SetActive(false);
        getRewardButton.gameObject.SetActive(false);

        currentQuest = null;
    }
}
