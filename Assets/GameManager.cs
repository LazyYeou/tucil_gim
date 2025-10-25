using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    public int currentScore = 0;
    private bool isGameActive = true;

    [Header("UI References")]
    public Text scoreText;
    public GameObject gameOverPanel;
    public Text finalScoreText;
    public GameObject levelCompletePanel;
    public Text levelScoreText;
    public GameObject leaderboardPanel;
    public Transform leaderboardContent;
    public GameObject leaderboardEntryPrefab;
    public InputField playerNameInput;

    [Header("References")]
    public PlayerController player;
    public CarSpawner carSpawner;
    public CollectibleSpawner collectibleSpawner;

    void Awake()
    {
        // Singleton pattern
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
        ResetGame();
    }

    void Update()
    {
        // Update score display
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore;
        }
    }

    public void AddScore(int points)
    {
        if (!isGameActive) return;
        currentScore += points;
    }

    public void GameOver()
    {
        Debug.Log("KELARLU");
        isGameActive = false;
        carSpawner?.StopSpawning();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText != null)
            {
                finalScoreText.text = "Final Score: " + currentScore;
            }
        }
    }

    public void LevelComplete()
    {
        isGameActive = false;
        carSpawner?.StopSpawning();

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
            if (levelScoreText != null)
            {
                levelScoreText.text = "Score: " + currentScore;
            }
        }
    }

    public void SubmitScore()
    {
        string playerName = playerNameInput != null ? playerNameInput.text : "Player";
        if (string.IsNullOrEmpty(playerName))
        {
            playerName = "Anonymous";
        }

        LeaderboardManager.Instance.AddScore(playerName, currentScore);
        ShowLeaderboard();
    }

    public void ShowLeaderboard()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        if (leaderboardPanel != null) leaderboardPanel.SetActive(true);

        UpdateLeaderboardDisplay();
    }

    void UpdateLeaderboardDisplay()
    {
        // Clear existing entries
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        // Get sorted scores
        List<LeaderboardEntry> entries = LeaderboardManager.Instance.GetTopScores(10);

        // Create UI entries
        for (int i = 0; i < entries.Count; i++)
        {
            GameObject entry = Instantiate(leaderboardEntryPrefab, leaderboardContent);
            Text entryText = entry.GetComponent<Text>();
            if (entryText != null)
            {
                entryText.text = $"{i + 1}. {entries[i].playerName} - {entries[i].score}";
            }
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ResetGame()
    {
        currentScore = 0;
        isGameActive = true;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        if (leaderboardPanel != null) leaderboardPanel.SetActive(false);

        player?.ResetPlayer();
        carSpawner?.StartSpawning();
        collectibleSpawner?.SpawnCollectibles();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}