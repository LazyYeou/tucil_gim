using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Crop", menuName = "Farming/Crop")]
public class CropData : ScriptableObject
{
    public string cropName;
    public Sprite seedSprite;
    public Sprite wateredSprite;
    public Sprite grownSprite;
    public float growthTime; // in seconds (30-300)
    public int sellPrice;
}

[System.Serializable]
public class Crop
{
    public CropData data;
    public bool isPlanted;
    public bool isWatered;
    public bool isGrown;
    public DateTime plantTime;
    public DateTime waterTime;

    public float GetGrowthProgress()
    {
        if (!isWatered) return 0f;

        TimeSpan elapsed = DateTime.Now - waterTime;
        float progress = (float)elapsed.TotalSeconds / data.growthTime;
        return Mathf.Clamp01(progress);
    }

    public bool CheckIfGrown()
    {
        if (!isWatered || isGrown) return isGrown;

        TimeSpan elapsed = DateTime.Now - waterTime;
        if (elapsed.TotalSeconds >= data.growthTime)
        {
            isGrown = true;
            return true;
        }
        return false;
    }
}

public class FarmPlot : MonoBehaviour
{
    public Crop currentCrop;
    public SpriteRenderer spriteRenderer;
    public GameObject timerUI;
    public TMPro.TextMeshProUGUI timerText;

    private FarmManager farmManager;

    void Start()
    {
        // Use the newer API to avoid obsolete FindObjectOfType
        farmManager = FindFirstObjectByType<FarmManager>();
        if (timerUI != null) timerUI.SetActive(false);
    }

    void Update()
    {
        if (currentCrop != null && currentCrop.isWatered && !currentCrop.isGrown)
        {
            currentCrop.CheckIfGrown();
            UpdateVisuals();
            UpdateTimer();
        }
    }

    public void PlantCrop(CropData cropData)
    {
        if (currentCrop != null && currentCrop.isPlanted) return;

        currentCrop = new Crop
        {
            data = cropData,
            isPlanted = true,
            isWatered = false,
            isGrown = false,
            plantTime = DateTime.Now
        };

        UpdateVisuals();
    }

    public void WaterCrop()
    {
        if (currentCrop == null || !currentCrop.isPlanted || currentCrop.isWatered) return;

        currentCrop.isWatered = true;
        currentCrop.waterTime = DateTime.Now;
        UpdateVisuals();

        if (timerUI != null) timerUI.SetActive(true);
    }

    public void HarvestCrop()
    {
        if (currentCrop == null || !currentCrop.isGrown) return;

        farmManager.AddCropToInventory(currentCrop.data);

        currentCrop = null;
        UpdateVisuals();
        if (timerUI != null) timerUI.SetActive(false);
    }

    public void UpdateVisuals()
    {
        if (currentCrop == null)
        {
            spriteRenderer.sprite = null;
            spriteRenderer.color = new Color(1, 1, 1, 0);
            return;
        }

        spriteRenderer.color = Color.white;

        if (currentCrop.isGrown)
        {
            spriteRenderer.sprite = currentCrop.data.grownSprite;
        }
        else if (currentCrop.isWatered)
        {
            spriteRenderer.sprite = currentCrop.data.wateredSprite;
        }
        else
        {
            spriteRenderer.sprite = currentCrop.data.seedSprite;
        }
    }

    void UpdateTimer()
    {
        if (timerText == null || currentCrop == null) return;

        if (currentCrop.isGrown)
        {
            timerText.text = "Ready!";
            return;
        }

        float remaining = currentCrop.data.growthTime - (float)(DateTime.Now - currentCrop.waterTime).TotalSeconds;

        if (remaining <= 0)
        {
            timerText.text = "Ready!";
        }
        else
        {
            int minutes = Mathf.FloorToInt(remaining / 60f);
            int seconds = Mathf.FloorToInt(remaining % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    void OnMouseDown()
    {
        if (farmManager == null) return;

        if (currentCrop == null)
        {
            if (farmManager.selectedCrop != null)
            {
                PlantCrop(farmManager.selectedCrop);
            }
        }
        else if (!currentCrop.isWatered)
        {
            WaterCrop();
        }
        else if (currentCrop.isGrown)
        {
            HarvestCrop();
        }
    }
}