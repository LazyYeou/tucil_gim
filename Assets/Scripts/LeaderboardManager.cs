using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;
    public string date;

    public LeaderboardEntry(string name, int s)
    {
        playerName = name;
        score = s;
        date = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    }
}

[System.Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
}

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    private const string LEADERBOARD_KEY = "RoadRushLeaderboard";
    private LeaderboardData leaderboardData;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLeaderboard();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(string playerName, int score)
    {
        LeaderboardEntry newEntry = new LeaderboardEntry(playerName, score);
        leaderboardData.entries.Add(newEntry);

        // Sort by score descending
        leaderboardData.entries = leaderboardData.entries
            .OrderByDescending(e => e.score)
            .ToList();

        // Keep only top 100 scores
        if (leaderboardData.entries.Count > 100)
        {
            leaderboardData.entries = leaderboardData.entries.Take(100).ToList();
        }

        SaveLeaderboard();
    }

    public List<LeaderboardEntry> GetTopScores(int count)
    {
        return leaderboardData.entries.Take(count).ToList();
    }

    public int GetPlayerRank(int score)
    {
        for (int i = 0; i < leaderboardData.entries.Count; i++)
        {
            if (score >= leaderboardData.entries[i].score)
            {
                return i + 1;
            }
        }
        return leaderboardData.entries.Count + 1;
    }

    void SaveLeaderboard()
    {
        string json = JsonUtility.ToJson(leaderboardData);
        PlayerPrefs.SetString(LEADERBOARD_KEY, json);
        PlayerPrefs.Save();
    }

    void LoadLeaderboard()
    {
        if (PlayerPrefs.HasKey(LEADERBOARD_KEY))
        {
            string json = PlayerPrefs.GetString(LEADERBOARD_KEY);
            leaderboardData = JsonUtility.FromJson<LeaderboardData>(json);
        }
        else
        {
            leaderboardData = new LeaderboardData();
        }
    }

    public void ClearLeaderboard()
    {
        leaderboardData.entries.Clear();
        SaveLeaderboard();
    }
}