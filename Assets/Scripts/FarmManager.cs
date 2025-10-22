using UnityEngine;
using System.Collections.Generic;

public class FarmManager : MonoBehaviour
{
    public static FarmManager Instance;

    [Header("Player Stats")]
    public int money = 100;
    public int experience = 0;
    public int level = 1;

    [Header("Inventory")]
    public Dictionary<string, int> cropInventory = new Dictionary<string, int>();

    [Header("Current Selection")]
    public CropData selectedCrop;

    [Header("UI References")]
    public TMPro.TextMeshProUGUI moneyText;
    public TMPro.TextMeshProUGUI expText;
    public TMPro.TextMeshProUGUI levelText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateUI();
    }

    public void AddCropToInventory(CropData crop)
    {
        if (cropInventory.ContainsKey(crop.cropName))
        {
            cropInventory[crop.cropName]++;
        }
        else
        {
            cropInventory[crop.cropName] = 1;
        }

        Debug.Log($"Harvested {crop.cropName}! Total: {cropInventory[crop.cropName]}");
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateUI();
    }

    public void AddExperience(int amount)
    {
        experience += amount;

        // Simple leveling system
        int expNeeded = level * 100;
        while (experience >= expNeeded)
        {
            experience -= expNeeded;
            level++;
            expNeeded = level * 100;
            Debug.Log($"Level Up! Now level {level}");
        }

        UpdateUI();
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    public bool HasCrop(string cropName, int amount)
    {
        return cropInventory.ContainsKey(cropName) && cropInventory[cropName] >= amount;
    }

    public bool RemoveCrop(string cropName, int amount)
    {
        if (HasCrop(cropName, amount))
        {
            cropInventory[cropName] -= amount;
            return true;
        }
        return false;
    }

    public void SelectCrop(CropData crop)
    {
        selectedCrop = crop;
        Debug.Log($"Selected crop: {crop.cropName}");
    }

    void UpdateUI()
    {
        if (moneyText != null)
            moneyText.text = $"Money: ${money}";

        if (expText != null)
        {
            int expNeeded = level * 100;
            expText.text = $"EXP: {experience}/{expNeeded}";
        }

        if (levelText != null)
            levelText.text = $"Level: {level}";
    }
}