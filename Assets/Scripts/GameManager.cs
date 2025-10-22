using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    public int score { get; private set; }
    public int collectedTrash { get; private set; }


    private float timer;
    private bool gameRunning;


    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }


    void Start()
    {
        StartLevel();
    }


    public void StartLevel()
    {
        score = 0;
        collectedTrash = 0;
        timer = LevelManager.Instance.levelTime;
        gameRunning = true;
        UIManager.Instance.UpdateScore(score);
        UIManager.Instance.UpdateTimer(timer);
        LevelManager.Instance.SpawnTrash();
    }


    void Update()
    {
        if (!gameRunning) return;
        timer -= Time.deltaTime;
        UIManager.Instance.UpdateTimer(timer);
        if (timer <= 0f)
        {
            Lose();
        }
    }


    public void AddScore(int value)
    {
        score += value;
        UIManager.Instance.UpdateScore(score);
    }


    public void OnTrashCollected(Trash t)
    {
        collectedTrash++;
        if (collectedTrash >= LevelManager.Instance.currentTrashCount)
        {
            Win();
        }
    }


    void Win()
    {
        gameRunning = false;
        UIManager.Instance.ShowEndScreen(true, score);
        LevelManager.Instance.NextLevel();
    }


    void Lose()
    {
        gameRunning = false;
        UIManager.Instance.ShowEndScreen(false, score);
    }
}