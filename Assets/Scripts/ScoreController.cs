using UnityEngine;
using TMPro;

public class ScoreController : MonoBehaviour
{
    public static ScoreController instance;
    
    [Header("UI Settings")]
    public TextMeshProUGUI scoreText; 

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        PlayerData.currentScore += amount;
        
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + PlayerData.currentScore.ToString();
        }
    }
}