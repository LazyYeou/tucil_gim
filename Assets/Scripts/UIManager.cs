using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }


    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public GameObject endScreen;
    public Text endMessage;


    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }


    public void UpdateScore(int score)
    {
        if (scoreText) scoreText.text = "Score: " + score;
    }


    public void UpdateTimer(float time)
    {
        if (timerText) timerText.text = "Time: " + Mathf.CeilToInt(time);
    }


    public void ShowEndScreen(bool win, int score)
    {
        endScreen.SetActive(true);
        endMessage.text = win ? "Stage Cleared! Score: " + score : "Time's Up!";
    }


    public void ShowGameComplete()
    {
        endScreen.SetActive(true);
        endMessage.text = "All Stages Complete!";
    }
}