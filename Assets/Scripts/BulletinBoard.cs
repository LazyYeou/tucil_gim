using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Quest
{
    public string questName;
    public string description;
    public string requiredCrop;
    public int requiredAmount;
    public int moneyReward;
    public int expReward;
    public bool isCompleted;

    public Quest(string name, string desc, string crop, int amount, int money, int exp)
    {
        questName = name;
        description = desc;
        requiredCrop = crop;
        requiredAmount = amount;
        moneyReward = money;
        expReward = exp;
        isCompleted = false;
    }
}

public class BulletinBoard : MonoBehaviour
{
    [Header("Quest List")]
    public List<Quest> activeQuests = new List<Quest>();

    [Header("UI References")]
    public Transform questContainer;
    public GameObject questUIPrefab;
    public GameObject bulletinPanel;

    private FarmManager farmManager;

    void Start()
    {
        farmManager = FarmManager.Instance;

        if (bulletinPanel != null)
            bulletinPanel.SetActive(false);

        GenerateInitialQuests();
    }

    void GenerateInitialQuests()
    {
        // Example quests - you can customize these
        activeQuests.Add(new Quest(
            "Fresh Tomatoes Needed",
            "The local restaurant needs fresh tomatoes!",
            "Tomato",
            3,
            50,
            25
        ));

        activeQuests.Add(new Quest(
            "Carrot Supply",
            "Deliver carrots for the market.",
            "Carrot",
            5,
            75,
            40
        ));

        activeQuests.Add(new Quest(
            "Wheat Harvest",
            "The bakery is running low on wheat.",
            "Wheat",
            10,
            120,
            60
        ));

        RefreshQuestUI();
    }

    public void OpenBulletinBoard()
    {
        if (bulletinPanel != null)
        {
            bulletinPanel.SetActive(true);
            RefreshQuestUI();
        }
    }

    public void CloseBulletinBoard()
    {
        if (bulletinPanel != null)
            bulletinPanel.SetActive(false);
    }

    void RefreshQuestUI()
    {
        // Clear existing UI
        foreach (Transform child in questContainer)
        {
            Destroy(child.gameObject);
        }

        // Create UI for each quest
        foreach (Quest quest in activeQuests)
        {
            if (!quest.isCompleted)
            {
                CreateQuestUI(quest);
            }
        }
    }

    void CreateQuestUI(Quest quest)
    {
        if (questUIPrefab == null || questContainer == null) return;

        GameObject questObj = Instantiate(questUIPrefab, questContainer);
        QuestUI questUI = questObj.GetComponent<QuestUI>();

        if (questUI != null)
        {
            questUI.SetupQuest(quest, this);
        }
    }

    public void CompleteQuest(Quest quest)
    {
        if (quest.isCompleted) return;

        // Check if player has required crops
        if (farmManager.HasCrop(quest.requiredCrop, quest.requiredAmount))
        {
            // Remove crops from inventory
            farmManager.RemoveCrop(quest.requiredCrop, quest.requiredAmount);

            // Give rewards
            farmManager.AddMoney(quest.moneyReward);
            farmManager.AddExperience(quest.expReward);

            // Mark as completed
            quest.isCompleted = true;

            Debug.Log($"Quest '{quest.questName}' completed! +${quest.moneyReward} +{quest.expReward}XP");

            // Refresh UI
            RefreshQuestUI();

            // Generate new quest
            GenerateNewQuest();
        }
        else
        {
            Debug.Log($"Not enough {quest.requiredCrop}! Need {quest.requiredAmount}");
        }
    }

    void GenerateNewQuest()
    {
        string[] cropTypes = { "Tomato", "Carrot", "Wheat", "Corn", "Potato" };
        string[] questTitles = { "Farm Fresh Request", "Market Supply", "Restaurant Order", "Community Need", "Harvest Request" };

        string crop = cropTypes[Random.Range(0, cropTypes.Length)];
        int amount = Random.Range(3, 15);
        int money = amount * Random.Range(15, 25);
        int exp = amount * Random.Range(8, 15);

        Quest newQuest = new Quest(
            questTitles[Random.Range(0, questTitles.Length)],
            $"Deliver {amount} {crop} to help the community!",
            crop,
            amount,
            money,
            exp
        );

        activeQuests.Add(newQuest);
        RefreshQuestUI();
    }

    void OnMouseDown()
    {
        OpenBulletinBoard();
    }
}