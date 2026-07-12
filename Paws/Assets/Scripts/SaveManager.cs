/*
Author: Muqrie Rahimi
Student ID: 1211109977
Date Created: 11 July 2026
*/
using UnityEngine;

// Dedicated save/load layer. ALL persistent game data goes through here (no other
// script touches PlayerPrefs for saved data), so persistence lives in one place.
//
// Saved data points (Pair tier - 2 values):
//   1. High Score          - best score achieved across sessions.
//   2. Total Fish Collected - cumulative number of fish picked up across ALL sessions.
public static class SaveManager
{
    private const string HighScoreKey = "PawsHighScore";
    private const string TotalFishKey = "PawsTotalFish";

    // ---- High Score ----

    public static int LoadHighScore()
    {
        return PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    // Saves the score only if it beats the stored high score. Returns true if it did.
    public static bool TrySaveHighScore(int score)
    {
        if (score > LoadHighScore())
        {
            PlayerPrefs.SetInt(HighScoreKey, score);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }

    // ---- Total Fish Collected (cumulative across sessions) ----

    public static int LoadTotalFish()
    {
        return PlayerPrefs.GetInt(TotalFishKey, 0);
    }

    public static void AddFishToTotal(int amount)
    {
        PlayerPrefs.SetInt(TotalFishKey, LoadTotalFish() + amount);
        PlayerPrefs.Save();
    }

    // ---- Utility ----

    // Wipes saved progress (handy for a "reset save" option or testing).
    public static void ClearAll()
    {
        PlayerPrefs.DeleteKey(HighScoreKey);
        PlayerPrefs.DeleteKey(TotalFishKey);
        PlayerPrefs.Save();
    }
}
