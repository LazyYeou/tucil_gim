using UnityEngine;


public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }


    public int currentLevel = 1;
    public int maxLevel = 5;
    public float levelTime = 45f;
    public GameObject trashPrefab;
    public int[] trashCounts = { 5, 10, 15, 20, 25 };


    public int currentTrashCount { get; private set; }
    public Vector2 spawnAreaMin = new Vector2(-7, -4);
    public Vector2 spawnAreaMax = new Vector2(7, 4);


    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }


    public void SpawnTrash()
    {
        currentTrashCount = trashCounts[Mathf.Clamp(currentLevel - 1, 0, trashCounts.Length - 1)];
        for (int i = 0; i < currentTrashCount; i++)
        {
            Vector2 pos = new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );
            Instantiate(trashPrefab, pos, Quaternion.identity);
        }
    }


    public void NextLevel()
    {
        if (currentLevel < maxLevel)
        {
            currentLevel++;
            SaveData.SaveLevel(currentLevel);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        }
        else
        {
            UIManager.Instance.ShowGameComplete();
        }
    }
}