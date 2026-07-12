/*
Author: Muqrie Rahimi
Student ID: 1211109977
Date Created: 11 July 2026
*/
using UnityEngine;

// Tracks the player's score for the current level and persists the best score
// between sessions via PlayerPrefs. Score is driven mainly by how many fish are
// collected and how quickly the level is finished (time bonus on victory).
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Live Points")]
    [SerializeField] private int pointsPerFish = 200;
    [SerializeField] private int pointsPerBurger = 50;
    [SerializeField] private int damagePenalty = 100;

    [Header("Victory Bonuses")]
    [SerializeField] private int timeBonusPerSecond = 5;
    [SerializeField] private int allFishBonus = 2000;

    private int score;
    private int lastTimeBonus;
    private int lastAllFishBonus;
    private bool newHighScore;
    private bool wasVictory;
    private bool finalized;

    public int Score => score;
    public int HighScore => SaveManager.LoadHighScore();
    public int LastTimeBonus => lastTimeBonus;
    public int LastAllFishBonus => lastAllFishBonus;
    public bool NewHighScore => newHighScore;
    public bool WasVictory => wasVictory;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // Only remove the duplicate component (this shares the manager object).
            Destroy(this);
        }
    }

    public void AddFishPoints()
    {
        Add(pointsPerFish);
    }

    public void AddBurgerPoints()
    {
        Add(pointsPerBurger);
    }

    public void ApplyDamagePenalty()
    {
        Add(-damagePenalty);
    }

    private void Add(int amount)
    {
        if (finalized) return;
        score = Mathf.Max(0, score + amount);
    }

    // Called once when the level ends (win or lose). Applies victory bonuses,
    // then saves the high score if beaten.
    public void EndGame(bool victory, float timeRemaining, int uniqueFish, int totalFish)
    {
        if (finalized) return;
        wasVictory = victory;

        if (victory)
        {
            lastTimeBonus = Mathf.CeilToInt(Mathf.Max(0f, timeRemaining)) * timeBonusPerSecond;
            lastAllFishBonus = (totalFish > 0 && uniqueFish >= totalFish) ? allFishBonus : 0;
            score += lastTimeBonus + lastAllFishBonus;
        }

        newHighScore = SaveManager.TrySaveHighScore(score);

        finalized = true;
    }
}
