using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMPro.TextMeshProUGUI questNameText;
    public TMPro.TextMeshProUGUI descriptionText;
    public TMPro.TextMeshProUGUI requirementText;
    public TMPro.TextMeshProUGUI rewardText;
    public Button completeButton;

    private Quest currentQuest;
    private BulletinBoard bulletinBoard;

    public void SetupQuest(Quest quest, BulletinBoard board)
    {
        currentQuest = quest;
        bulletinBoard = board;

        if (questNameText != null)
            questNameText.text = quest.questName;

        if (descriptionText != null)
            descriptionText.text = quest.description;

        if (requirementText != null)
            requirementText.text = $"Need: {quest.requiredAmount}x {quest.requiredCrop}";

        if (rewardText != null)
            rewardText.text = $"Reward: ${quest.moneyReward} | {quest.expReward}XP";

        if (completeButton != null)
            completeButton.onClick.AddListener(OnCompleteClicked);

        UpdateButtonState();
    }

    void Update()
    {
        UpdateButtonState();
    }

    void UpdateButtonState()
    {
        if (completeButton != null && currentQuest != null)
        {
            FarmManager fm = FarmManager.Instance;
            bool canComplete = fm != null && fm.HasCrop(currentQuest.requiredCrop, currentQuest.requiredAmount);
            completeButton.interactable = canComplete;
        }
    }

    void OnCompleteClicked()
    {
        if (bulletinBoard != null && currentQuest != null)
        {
            bulletinBoard.CompleteQuest(currentQuest);
        }
    }
}